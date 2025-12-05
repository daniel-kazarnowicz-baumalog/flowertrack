/**
 * TypeScript types matching backend API contracts
 * Generated from Flowertrack.Contracts namespace
 */

// ============================================================================
// Common Types
// ============================================================================

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
  statusCode?: number;
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// ============================================================================
// Authentication Types
// ============================================================================

// Service User Authentication
export interface ServiceUserDto {
  id: string;
  email: string;
  fullName: string;
  role: string;
  status: string;
}

export interface LoginServiceUserRequest {
  email: string;
  password: string;
}

export interface LoginServiceUserResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string; // ISO date string
  user: ServiceUserDto;
}

export interface SignupServiceUserRequest {
  token: string;
  password: string;
}

export interface SignupServiceUserResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: ServiceUserDto;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}

// Organization User Authentication
export interface OrganizationUserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  organizationId: string;
  organizationName: string;
  isAdmin: boolean;
  status: string;
}

export interface LoginOrganizationUserRequest {
  email: string;
  password: string;
}

export interface LoginOrganizationUserResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: OrganizationUserDto;
}

export interface ActivateAccountRequest {
  token: string;
  password: string;
}

export interface ActivateAccountResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: OrganizationUserDto;
}

export interface InviteOrganizationUserRequest {
  organizationId: string;
  email: string;
  firstName: string;
  lastName: string;
}

export interface InviteOrganizationUserResponse {
  message: string;
  email: string;
}

// ============================================================================
// Ticket Types
// ============================================================================

export type TicketStatus = 'New' | 'Accepted' | 'InProgress' | 'Resolved' | 'Closed' | 'Reopened';

export type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';

export interface TicketDto {
  id: string;
  ticketNumber: string;
  title: string;
  description: string;
  status: TicketStatus;
  priority: TicketPriority;
  organizationId: string;
  organizationName: string;
  machineId: string;
  machineModel: string;
  machineSerialNumber: string;
  assignedToId?: string;
  assignedToName?: string;
  createdById: string;
  createdByName: string;
  createdAt: string;
  updatedAt: string;
  resolvedAt?: string;
  closedAt?: string;
}

export interface CreateTicketRequest {
  machineId: string;
  title: string;
  description: string;
  priority: TicketPriority;
}

export interface UpdateTicketRequest {
  title?: string;
  description?: string;
  priority?: TicketPriority;
}

export interface ChangeTicketStatusRequest {
  newStatus: TicketStatus;
  justification?: string;
}

export interface AssignTicketRequest {
  serviceUserId?: string; // null to unassign
}

export interface TicketFilters {
  status?: TicketStatus[];
  priority?: TicketPriority[];
  organizationId?: string;
  machineId?: string;
  assignedToId?: string;
  search?: string;
  page?: number;
  pageSize?: number;
}

export interface TicketGroupedByStatus {
  status: TicketStatus;
  count: number;
  tickets: TicketDto[];
}

// Ticket History/Timeline
export type TicketEventType =
  | 'Created'
  | 'StatusChanged'
  | 'Assigned'
  | 'Unassigned'
  | 'CommentAdded'
  | 'NoteAdded'
  | 'AttachmentAdded'
  | 'Updated';

export interface TicketHistoryEvent {
  id: string;
  ticketId: string;
  eventType: TicketEventType;
  description: string;
  performedByUserId: string;
  performedByUserName: string;
  performedByUserType: 'Service' | 'Organization';
  metadata?: Record<string, unknown>;
  createdAt: string;
}

// ============================================================================
// Comment Types
// ============================================================================

export interface CommentDto {
  id: string;
  ticketId: string;
  content: string;
  authorId: string;
  authorName: string;
  authorType: 'Service' | 'Organization';
  isInternal: boolean; // true for internal notes
  createdAt: string;
  updatedAt?: string;
}

export interface CreateCommentRequest {
  content: string;
}

export interface UpdateCommentRequest {
  content: string;
}

export interface CreateNoteRequest {
  content: string;
}

// ============================================================================
// Attachment Types
// ============================================================================

export interface AttachmentDto {
  id: string;
  ticketId: string;
  fileName: string;
  fileSize: number;
  contentType: string;
  uploadedByUserId: string;
  uploadedByUserName: string;
  uploadedByUserType: 'Service' | 'Organization';
  createdAt: string;
}

// ============================================================================
// Organization Types
// ============================================================================

export interface OrganizationDto {
  id: string;
  name: string;
  contactEmail: string;
  contactPhone?: string;
  address?: string;
  apiKey?: string; // Only visible to service admins
  machinesCount: number;
  activeTicketsCount: number;
  hasAlarmMachines: boolean;
  createdAt: string;
}

export interface OnboardOrganizationRequest {
  organizationName: string;
  adminEmail: string;
  adminFirstName: string;
  adminLastName: string;
  contactEmail?: string;
  contactPhone?: string;
  address?: string;
}

export interface UpdateOrganizationRequest {
  name?: string;
  contactEmail?: string;
  contactPhone?: string;
  address?: string;
}

export interface RegenerateApiKeyResponse {
  apiKey: string;
}

// ============================================================================
// Machine Types
// ============================================================================

export type MachineStatus = 'Active' | 'Maintenance' | 'Alarm' | 'Inactive';

export interface MachineDto {
  id: string;
  organizationId: string;
  organizationName: string;
  model: string;
  serialNumber: string;
  installationDate?: string;
  status: MachineStatus;
  location?: string;
  notes?: string;
  activeTicketsCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateMachineRequest {
  organizationId: string;
  model: string;
  serialNumber: string;
  installationDate?: string;
  location?: string;
  notes?: string;
}

export interface UpdateMachineRequest {
  model?: string;
  serialNumber?: string;
  installationDate?: string;
  location?: string;
  notes?: string;
}

export interface ChangeMachineStatusRequest {
  status: MachineStatus;
  reason?: string;
}

export interface MachineLogDto {
  id: string;
  machineId: string;
  eventType: string;
  description: string;
  performedByUserId?: string;
  performedByUserName?: string;
  createdAt: string;
}

// ============================================================================
// User Management Types
// ============================================================================

// Service Users
export interface ServiceUserDetailsDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  role: 'Admin' | 'Technician';
  isActive: boolean;
  lastActivityAt?: string;
  assignedTicketsCount: number;
  createdAt: string;
}

export interface InviteServiceUserRequest {
  email: string;
  firstName: string;
  lastName: string;
  isAdmin: boolean;
}

export interface UpdateServiceUserRequest {
  firstName?: string;
  lastName?: string;
  email?: string;
  isAdmin?: boolean;
}

export interface DeactivateServiceUserRequest {
  reason: string;
}

// Organization Users
export interface OrganizationUserDetailsDto {
  id: string;
  organizationId: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  isAdmin: boolean;
  isActive: boolean;
  lastActivityAt?: string;
  ticketsCreatedCount: number;
  createdAt: string;
}

// Team Management Types
export type UserRole = 'Admin' | 'User';
export type UserStatus = 'Active' | 'Invited' | 'Pending';

export interface OrganizationUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  status: UserStatus;
  joinedAt: string;
  organizationId: string;
}

export interface InviteTeamMemberRequest {
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
}

export interface UpdateMemberRoleRequest {
  role: UserRole;
}
