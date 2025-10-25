# REST API Plan for FLOWerTRACK

This document outlines the comprehensive REST API plan for the FLOWerTRACK application, based on the provided database schema, Product Requirements Document (PRD), and technology stack.

## 1. Resources

The API is designed around the following primary resources, which correspond to the main entities in the database schema:

-   **Tickets**: Corresponds to the `Tickets` table. Represents service tickets.
-   **Organizations**: Corresponds to the `Organizations` table. Represents client companies.
-   **Machines**: Corresponds to the `Machines` table. Represents equipment belonging to organizations.
-   **Users**: Represents both `ServiceUsers` and `OrganizationUsers`. User management is split between authentication concerns and profile data.
-   **Comments & Notes**: A sub-resource of `Tickets`, corresponding to entries in the `TicketHistory` table with event types 'COMMENT' or 'NOTE'.
-   **History**: A sub-resource of `Tickets`, corresponding to the `TicketHistory` table.
-   **Attachments**: Corresponds to the `Attachments` table. Represents files associated with tickets.
-   **Auth**: A virtual resource for handling authentication operations like login and password reset.
-   **MachineLogs**: Corresponds to the `MachineLogs` table. Represents logs received from machines.
-   **AuditLog**: Corresponds to the `AuditLog` table. For administrative review of system actions.
-   **Dictionary Data**: Virtual resources for retrieving values from dictionary tables (`TicketStatuses`, `TicketPriorities`, `Roles`, `MaintenanceIntervals`).

---

## 2. Endpoints

### 2.1. Auth Endpoints

-   **Method**: `POST`
-   **URL**: `/api/auth/login`
-   **Description**: Authenticates a user and returns a JWT.
-   **Request Body**:
    ```json
    {
      "email": "user@example.com",
      "password": "user_password"
    }
    ```
-   **Response Body**:
    ```json
    {
      "accessToken": "your_jwt_token",
      "expiresIn": 3600,
      "user": {
        "id": "uuid-of-user",
        "email": "user@example.com",
        "roles": ["ServiceTechnician"]
      }
    }
    ```
-   **Success**: `200 OK`
-   **Error**: `400 Bad Request`, `401 Unauthorized`

---

-   **Method**: `POST`
-   **URL**: `/api/auth/forgot-password`
-   **Description**: Initiates the password reset process for a user.
-   **Request Body**:
    ```json
    {
      "email": "user@example.com"
    }
    ```
-   **Success**: `202 Accepted` (Indicates the request has been accepted for processing)
-   **Error**: `400 Bad Request`, `404 Not Found`

---

### 2.2. Ticket Endpoints

-   **Method**: `GET`
-   **URL**: `/api/tickets`
-   **Description**: Retrieves a paginated list of tickets. Supports filtering and sorting.
-   **Query Parameters**:
    -   `page` (int, default: 1)
    -   `pageSize` (int, default: 20)
    -   `sortBy` (string, e.g., "createdAt")
    -   `sortOrder` (string, "asc" or "desc", default: "desc")
    -   `statusId` (int)
    -   `priorityId` (int)
    -   `organizationId` (uuid)
    -   `machineId` (uuid)
    -   `assignedTo` (uuid or "me" or "unassigned")
    -   `search` (string)
-   **Response Body**:
    ```json
    {
      "pagination": {
        "page": 1,
        "pageSize": 20,
        "totalItems": 150,
        "totalPages": 8
      },
      "data": [
        {
          "id": "uuid-of-ticket",
          "ticketNumber": 1001,
          "title": "Machine not responding",
          "status": "New",
          "priority": "High",
          "organizationName": "Client Corp",
          "machineSerialNumber": "SN-123",
          "assignedTo": "John Doe",
          "createdAt": "2025-10-25T20:48:58Z"
        }
      ]
    }
    ```
-   **Success**: `200 OK`
-   **Error**: `400 Bad Request`, `401 Unauthorized`

---

-   **Method**: `POST`
-   **URL**: `/api/tickets`
-   **Description**: Creates a new ticket. (US-042)
-   **Request Body**:
    ```json
    {
      "machineId": "uuid-of-machine",
      "title": "Emergency stop button is stuck",
      "description": "The main emergency stop button cannot be disengaged.",
      "priorityId": 4 // 'Critical'
    }
    ```
-   **Response Body**:
    ```json
    {
      "id": "new-uuid-of-ticket",
      "ticketNumber": 1002,
      "title": "Emergency stop button is stuck",
      "status": "New",
      "createdAt": "2025-10-25T21:00:00Z"
    }
    ```
-   **Success**: `201 Created`
-   **Error**: `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`

---

-   **Method**: `GET`
-   **URL**: `/api/tickets/{id}`
-   **Description**: Retrieves the full details of a single ticket. (US-015)
-   **Response Body**:
    ```json
    {
      "id": "uuid-of-ticket",
      "ticketNumber": 1001,
      "title": "Machine not responding",
      "description": "The machine is offline and not responding to pings.",
      "status": "In Progress",
      "priority": "High",
      "createdAt": "2025-10-25T20:48:58Z",
      "updatedAt": "2025-10-25T22:15:00Z",
      "organization": { "id": "uuid-org", "name": "Client Corp" },
      "machine": { "id": "uuid-machine", "serialNumber": "SN-123", "model": "Model-X" },
      "createdBy": { "id": "uuid-user", "name": "Jane Operator" },
      "assignedTo": { "id": "uuid-tech", "name": "John Doe" }
    }
    ```
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `404 Not Found`

---

-   **Method**: `PATCH`
-   **URL**: `/api/tickets/{id}`
-   **Description**: Partially updates a ticket (e.g., title, description).
-   **Request Body**:
    ```json
    {
      "title": "Updated: Machine not responding after restart"
    }
    ```
-   **Success**: `204 No Content`
-   **Error**: `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`

---

-   **Method**: `POST`
-   **URL**: `/api/tickets/{id}/assign`
-   **Description**: Assigns a ticket to a service technician. (US-019)
-   **Request Body**:
    ```json
    {
      "technicianId": "uuid-of-technician" // can be null to unassign
    }
    ```
-   **Success**: `204 No Content`
-   **Error**: `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`

---

-   **Method**: `POST`
-   **URL**: `/api/tickets/{id}/status`
-   **Description**: Changes the status of a ticket, enforcing workflow rules. (US-017)
-   **Request Body**:
    ```json
    {
      "newStatusId": 3, // 'In Progress'
      "justification": "Starting investigation." // Optional, required for some transitions
    }
    ```
-   **Success**: `204 No Content`
-   **Error**: `400 Bad Request` (e.g., invalid status transition), `401 Unauthorized`, `403 Forbidden`, `404 Not Found`

---

-   **Method**: `POST`
-   **URL**: `/api/tickets/{id}/reopen`
-   **Description**: Reopens a resolved ticket. (US-047)
-   **Request Body**:
    ```json
    {
      "reason": "The issue has reappeared."
    }
    ```
-   **Success**: `204 No Content`
-   **Error**: `400 Bad Request` (e.g., ticket is not 'Resolved' or 14-day window has passed), `401 Unauthorized`, `403 Forbidden`, `404 Not Found`

---

### 2.3. Ticket Sub-Resources (History, Comments, Attachments)

-   **Method**: `GET`
-   **URL**: `/api/tickets/{id}/history`
-   **Description**: Retrieves the complete event timeline for a ticket. (US-016)
-   **Response Body**:
    ```json
    [
      {
        "id": "uuid-history-event",
        "eventType": "STATUS_CHANGE",
        "details": { "from": "New", "to": "In Progress" },
        "user": { "id": "uuid-user", "name": "John Doe" },
        "createdAt": "2025-10-25T22:15:00Z"
      },
      {
        "id": "uuid-history-event-2",
        "eventType": "COMMENT",
        "details": { "text": "I have started looking into this." },
        "user": { "id": "uuid-user", "name": "John Doe" },
        "createdAt": "2025-10-25T22:16:00Z"
      }
    ]
    ```
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `404 Not Found`

---

-   **Method**: `POST`
-   **URL**: `/api/tickets/{id}/comments`
-   **Description**: Adds a public comment to a ticket. (US-020)
-   **Request Body**:
    ```json
    {
      "text": "Could you please provide the machine's log file from around 10:00 AM?"
    }
    ```
-   **Response Body**: (The created comment resource)
    ```json
    {
      "id": "uuid-of-new-comment-event",
      "eventType": "COMMENT",
      "details": { "text": "Could you please provide the machine's log file from around 10:00 AM?" },
      "user": { "id": "uuid-user", "name": "John Doe" },
      "createdAt": "2025-10-25T22:20:00Z"
    }
    ```
-   **Success**: `201 Created`
-   **Error**: `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`

---

-   **Method**: `POST`
-   **URL**: `/api/tickets/{id}/notes`
-   **Description**: Adds an internal-only note to a ticket. (US-018)
-   **Request Body**:
    ```json
    {
      "text": "Suspecting a faulty sensor. Will need to check part availability."
    }
    ```
-   **Success**: `201 Created`
-   **Error**: `400 Bad Request`, `401 Unauthorized`, `403 Forbidden` (Only service team can add notes), `404 Not Found`

---

-   **Method**: `GET`
-   **URL**: `/api/tickets/{id}/attachments`
-   **Description**: Lists all attachments for a ticket. (US-023)
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `404 Not Found`

---

-   **Method**: `POST`
-   **URL**: `/api/tickets/{id}/attachments`
-   **Description**: Adds an attachment to a ticket. This will be a multipart/form-data request.
-   **Success**: `201 Created`
-   **Error**: `400 Bad Request` (e.g., file too large), `401 Unauthorized`, `404 Not Found`

---

### 2.4. Organization Endpoints

-   **Method**: `GET`
-   **URL**: `/api/organizations`
-   **Description**: Retrieves a list of all organizations. (Service Portal only) (US-024)
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `403 Forbidden`

---

-   **Method**: `POST`
-   **URL**: `/api/organizations/onboard`
-   **Description**: Onboards a new organization and sends an invitation to their admin. (Service Admin only) (US-025)
-   **Request Body**:
    ```json
    {
      "name": "New Client Inc.",
      "adminEmail": "admin@newclient.com",
      "adminFirstName": "Eva",
      "adminLastName": "Smith"
    }
    ```
-   **Success**: `202 Accepted`
-   **Error**: `400 Bad Request` (e.g., email already exists), `401 Unauthorized`, `403 Forbidden`

---

-   **Method**: `GET`
-   **URL**: `/api/organizations/{id}`
-   **Description**: Retrieves details for a single organization.
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `403 Forbidden`, `404 Not Found`

---

-   **Method**: `GET`
-   **URL**: `/api/organizations/{id}/machines`
-   **Description**: Retrieves a list of machines for a specific organization.
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `403 Forbidden`, `404 Not Found`

---

-   **Method**: `POST`
-   **URL**: `/api/organizations/{id}/token/regenerate`
-   **Description**: Regenerates the machine API token for an organization. (Service Admin only) (US-028)
-   **Response Body**:
    ```json
    {
      "apiToken": "newly-generated-api-token"
    }
    ```
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `403 Forbidden`, `404 Not Found`

---

### 2.5. Machine Endpoints

-   **Method**: `GET`
-   **URL**: `/api/machines`
-   **Description**: Retrieves a list of all machines across all organizations. (Service Portal only)
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `403 Forbidden`

---

-   **Method**: `POST`
-   **URL**: `/api/machines`
-   **Description**: Adds a new machine to an organization. (Service Admin only) (US-027)
-   **Request Body**:
    ```json
    {
      "organizationId": "uuid-of-organization",
      "serialNumber": "SN-XYZ-987",
      "brand": "Baumalog",
      "model": "Tower-2.0",
      "location": "Warehouse B"
    }
    ```
-   **Success**: `201 Created`
-   **Error**: `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`

---

-   **Method**: `GET`
-   **URL**: `/api/machines/{id}`
-   **Description**: Retrieves details for a single machine.
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `404 Not Found`

---

### 2.6. User Management Endpoints (Admin)

-   **Method**: `GET`
-   **URL**: `/api/admin/users`
-   **Description**: Retrieves a list of all users (both service and organization). (Service Admin only) (US-031, US-049)
-   **Query Parameters**: `role`, `organizationId`
-   **Success**: `200 OK`
-   **Error**: `401 Unauthorized`, `403 Forbidden`

---

-   **Method**: `POST`
-   **URL**: `/api/admin/users/invite`
-   **Description**: Invites a new user to the system (either service or organization). (US-032, US-050)
-   **Request Body**:
    ```json
    {
      "email": "new.user@example.com",
      "firstName": "Test",
      "lastName": "User",
      "roleId": 1, // 'ServiceTechnician'
      "organizationId": null // or uuid if it's an organization user
    }
    ```
-   **Success**: `202 Accepted`
-   **Error**: `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`

---

### 2.7. Machine Log Ingestion Endpoint

-   **Method**: `POST`
-   **URL**: `/api/ingest/logs`
-   **Description**: Public-facing endpoint for machines to send logs. Authenticated via API Token in header.
-   **Request Header**: `X-API-Token: <machine_api_token>`
-   **Request Body**:
    ```json
    {
      "timestamp": "2025-10-26T10:00:00Z",
      "status": "ALARM",
      "telemetry": {
        "temperature": 95.5,
        "pressure": 300
      },
      "alarms": [
        { "code": "E-101", "message": "Over-temperature warning" }
      ]
    }
    ```
-   **Success**: `202 Accepted` (The log is accepted for asynchronous processing)
-   **Error**: `400 Bad Request`, `401 Unauthorized` (Invalid or missing token), `429 Too Many Requests`

---

## 3. Authentication and Authorization

-   **Authentication**: The API will use **JSON Web Tokens (JWT)** for authentication. The user will send their credentials (`email`, `password`) to the `/api/auth/login` endpoint. Upon successful authentication, the server will return a short-lived JWT `accessToken`. This token must be included in the `Authorization` header of all subsequent requests as a Bearer token (`Authorization: Bearer <token>`).
-   **Authorization**: Authorization will be role-based (RBAC), as defined in the PRD (US-053). The user's roles will be encoded into the JWT payload. The .NET API backend will use middleware and attributes (e.g., `[Authorize(Roles = "ServiceAdministrator")]`) to protect endpoints. This server-side check is critical and will be the source of truth for access control, supplemented by PostgreSQL Row-Level Security (RLS) for an additional layer of data protection as described in `db-plan.md`.

---

## 4. Validation and Business Logic

-   **Validation**:
    -   All incoming request bodies (DTOs) will be validated at the API layer before processing.
    -   This includes checks for required fields (`NOT NULL`), data types, string lengths (`VARCHAR(255)`), and formats (e.g., valid email).
    -   Business-specific rules, such as minimum title length (US-042), will also be enforced.
    -   Uniqueness constraints (`UNIQUE` in the DB schema, e.g., machine `serial_number`) will be checked. A `409 Conflict` error will be returned if a unique constraint is violated.

-   **Business Logic Implementation**:
    -   **Ticket Status Workflow (US-017)**: The `POST /api/tickets/{id}/status` endpoint will contain the logic to validate state transitions. It will reject invalid changes (e.g., from 'New' to 'Resolved') with a `400 Bad Request` error.
    -   **Ticket Reopening (US-047)**: The `POST /api/tickets/{id}/reopen` endpoint will check if the ticket is in the 'Resolved' state and if the current date is within 14 days of the `resolved_at` timestamp.
    -   **Internal Notes (US-018)**: The `POST /api/tickets/{id}/notes` endpoint is distinct from the `/comments` endpoint and will be restricted to service team roles. The created `TicketHistory` event will be marked in a way that RLS policies can hide it from clients.
    -   **Onboarding (US-025, US-050)**: The onboarding/invite endpoints will create user and organization records and then trigger an external email service to send an invitation link with a temporary, expiring token.
    -   **Asynchronous Log Ingestion**: The `/api/ingest/logs` endpoint will perform only essential validation (token, basic structure) and then place the log content onto a message queue for asynchronous processing by a background worker. This ensures the endpoint is highly available and responsive, as per the recommendation in `db-planning-session.md`.
```