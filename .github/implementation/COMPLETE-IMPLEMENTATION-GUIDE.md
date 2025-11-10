# 🚀 Complete Implementation Guide - FLOWerTRACK Frontend MVP

This guide provides step-by-step instructions to complete the remaining 70% of the frontend implementation.

---

## 🎯 Implementation Order (By Priority)

Follow this order to build features in the correct dependency sequence:

1. **Organizations Management** (Week 1) - Foundation for everything
2. **Machines Management** (Week 1) - Required for tickets
3. **Advanced Form Components** (Week 1) - Needed for forms
4. **Tickets Management** (Week 2-3) - Core business logic
5. **Comments & Attachments** (Week 3) - Communication
6. **Timeline** (Week 3) - Audit trail
7. **Dashboards & KPIs** (Week 4) - Analytics
8. **Team Management** (Week 4) - User admin
9. **Advanced Components** (Week 5) - Polish
10. **Testing & Deployment** (Week 6) - Quality & Launch

---

## 📁 File Structure Reference

```
src/frontend/flowertrack-client/src/
├── components/
│   ├── auth/
│   │   └── ProtectedRoute.tsx ✅
│   ├── layout/
│   │   ├── ServiceLayout.tsx ✅
│   │   ├── ClientLayout.tsx ✅
│   │   ├── MainLayout.tsx ✅
│   │   └── Navbar.tsx ✅
│   ├── ui/
│   │   ├── Button.tsx ✅
│   │   ├── Input.tsx ✅
│   │   ├── Card.tsx ✅
│   │   ├── Badge.tsx ✅
│   │   ├── Modal.tsx ✅
│   │   ├── Loader.tsx ✅
│   │   ├── Toast.tsx ✅
│   │   ├── Select.tsx ⏳ TODO
│   │   ├── Textarea.tsx ⏳ TODO
│   │   ├── DatePicker.tsx ⏳ TODO
│   │   ├── Table.tsx ⏳ TODO
│   │   ├── SkeletonLoader.tsx ⏳ TODO
│   │   ├── EmptyState.tsx ⏳ TODO
│   │   └── ErrorState.tsx ⏳ TODO
│   ├── organizations/ ⏳ TODO
│   │   ├── OrganizationList.tsx
│   │   ├── OrganizationDetails.tsx
│   │   ├── OrganizationForm.tsx
│   │   ├── OnboardOrganizationModal.tsx
│   │   └── OrganizationCard.tsx
│   ├── machines/ ⏳ TODO
│   │   ├── MachineList.tsx
│   │   ├── MachineDetails.tsx
│   │   ├── MachineForm.tsx
│   │   ├── MachineStatusBadge.tsx
│   │   └── MachineLogsViewer.tsx
│   ├── tickets/ ⏳ TODO
│   │   ├── TicketList.tsx
│   │   ├── TicketTable.tsx
│   │   ├── TicketDetails.tsx
│   │   ├── TicketForm.tsx
│   │   ├── TicketFilters.tsx
│   │   ├── TicketStatusManager.tsx
│   │   ├── TicketAssignment.tsx
│   │   └── TicketTimeline.tsx
│   ├── comments/ ⏳ TODO
│   │   ├── CommentsList.tsx
│   │   ├── CommentItem.tsx
│   │   ├── AddCommentForm.tsx
│   │   ├── EditCommentModal.tsx
│   │   └── InternalNotesSection.tsx
│   ├── attachments/ ⏳ TODO
│   │   ├── AttachmentsList.tsx
│   │   ├── AttachmentItem.tsx
│   │   ├── FileUpload.tsx
│   │   └── DownloadButton.tsx
│   ├── dashboard/ ⏳ TODO
│   │   ├── KPICard.tsx
│   │   ├── ActivityFeed.tsx
│   │   ├── LineChart.tsx
│   │   ├── PieChart.tsx
│   │   └── BarChart.tsx
│   └── users/ ⏳ TODO
│       ├── UsersList.tsx
│       ├── UserForm.tsx
│       ├── InviteUserModal.tsx
│       └── ManageUserModal.tsx
├── pages/
│   ├── service/
│   │   ├── ServiceLoginPage.tsx ✅
│   │   ├── ServiceDashboard.tsx ✅ (skeleton)
│   │   ├── OrganizationsPage.tsx ⏳ TODO
│   │   ├── OrganizationDetailsPage.tsx ⏳ TODO
│   │   ├── TicketsPage.tsx ⏳ TODO
│   │   ├── TicketDetailsPage.tsx ⏳ TODO
│   │   ├── MachinesPage.tsx ⏳ TODO
│   │   ├── MachineDetailsPage.tsx ⏳ TODO
│   │   └── AdminUsersPage.tsx ⏳ TODO
│   ├── client/
│   │   ├── ClientLoginPage.tsx ✅
│   │   ├── ClientDashboard.tsx ✅ (skeleton)
│   │   ├── TicketsPage.tsx ⏳ TODO
│   │   ├── TicketDetailsPage.tsx ⏳ TODO
│   │   ├── CreateTicketPage.tsx ⏳ TODO
│   │   └── TeamManagementPage.tsx ⏳ TODO
│   └── NotFound.tsx ✅
├── services/
│   ├── api.ts ✅ (client configured)
│   ├── organizationService.ts ⏳ TODO
│   ├── machineService.ts ⏳ TODO
│   ├── ticketService.ts ⏳ TODO
│   ├── commentService.ts ⏳ TODO
│   ├── attachmentService.ts ⏳ TODO
│   └── userService.ts ⏳ TODO
├── hooks/
│   ├── useToast.ts ✅
│   ├── useOrganizations.ts ⏳ TODO
│   ├── useMachines.ts ⏳ TODO
│   ├── useTickets.ts ⏳ TODO
│   └── useUsers.ts ⏳ TODO
├── contexts/
│   ├── AuthContext.tsx ✅
│   └── ToastContext.tsx ✅
├── types/
│   └── api.ts ✅
└── lib/
    └── env.ts ✅
```

---

## 🔨 PHASE 1: Organizations Management (Week 1)

### Step 1.1: Create Organization Service

**File:** `src/services/organizationService.ts`

```typescript
import api from './api';
import { Organization, CreateOrganizationDto, UpdateOrganizationDto } from '../types/api';

export const organizationService = {
  // List all organizations
  async getAll(): Promise<Organization[]> {
    const response = await api.get('/api/organizations');
    return response.data;
  },

  // Get organization by ID
  async getById(id: string): Promise<Organization> {
    const response = await api.get(`/api/organizations/${id}`);
    return response.data;
  },

  // Onboard new organization
  async onboard(data: CreateOrganizationDto): Promise<Organization> {
    const response = await api.post('/api/organizations/onboard', data);
    return response.data;
  },

  // Update organization
  async update(id: string, data: UpdateOrganizationDto): Promise<Organization> {
    const response = await api.patch(`/api/organizations/${id}`, data);
    return response.data;
  },

  // Regenerate API key
  async regenerateApiKey(id: string): Promise<{ apiKey: string }> {
    const response = await api.post(`/api/organizations/${id}/regenerate-api-key`);
    return response.data;
  },

  // Get organization machines
  async getMachines(id: string): Promise<Machine[]> {
    const response = await api.get(`/api/organizations/${id}/machines`);
    return response.data;
  },

  // Get organization tickets
  async getTickets(id: string): Promise<Ticket[]> {
    const response = await api.get(`/api/organizations/${id}/tickets`);
    return response.data;
  },

  // Get organization users
  async getUsers(id: string): Promise<OrganizationUser[]> {
    const response = await api.get(`/api/organizations/${id}/users`);
    return response.data;
  },
};
```

### Step 1.2: Create React Query Hook

**File:** `src/hooks/useOrganizations.ts`

```typescript
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { organizationService } from '../services/organizationService';
import { useToast } from './useToast';

export const useOrganizations = () => {
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  // List all organizations
  const { data: organizations, isLoading, error } = useQuery({
    queryKey: ['organizations'],
    queryFn: organizationService.getAll,
  });

  // Create organization
  const createMutation = useMutation({
    mutationFn: organizationService.onboard,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['organizations'] });
      showToast('Organization onboarded successfully', 'success');
    },
    onError: (error: any) => {
      showToast(error.response?.data?.message || 'Failed to onboard organization', 'error');
    },
  });

  // Update organization
  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateOrganizationDto }) => 
      organizationService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['organizations'] });
      showToast('Organization updated successfully', 'success');
    },
    onError: (error: any) => {
      showToast(error.response?.data?.message || 'Failed to update organization', 'error');
    },
  });

  return {
    organizations,
    isLoading,
    error,
    createOrganization: createMutation.mutate,
    updateOrganization: updateMutation.mutate,
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
  };
};

export const useOrganization = (id: string) => {
  const { data: organization, isLoading, error } = useQuery({
    queryKey: ['organizations', id],
    queryFn: () => organizationService.getById(id),
    enabled: !!id,
  });

  return { organization, isLoading, error };
};
```

### Step 1.3: Create Organization List Component

**File:** `src/components/organizations/OrganizationList.tsx`

```typescript
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useOrganizations } from '../../hooks/useOrganizations';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { Badge } from '../ui/Badge';
import { Loader } from '../ui/Loader';
import { EmptyState } from '../ui/EmptyState';
import { ErrorState } from '../ui/ErrorState';
import { OnboardOrganizationModal } from './OnboardOrganizationModal';
import styles from './OrganizationList.module.css';

export const OrganizationList: React.FC = () => {
  const navigate = useNavigate();
  const { organizations, isLoading, error } = useOrganizations();
  const [searchTerm, setSearchTerm] = useState('');
  const [showOnboardModal, setShowOnboardModal] = useState(false);

  if (isLoading) return <Loader />;
  if (error) return <ErrorState message="Failed to load organizations" onRetry={() => window.location.reload()} />;
  if (!organizations || organizations.length === 0) {
    return <EmptyState message="No organizations yet" action={
      <Button onClick={() => setShowOnboardModal(true)}>Onboard Organization</Button>
    } />;
  }

  const filteredOrganizations = organizations.filter(org =>
    org.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1>Organizations</h1>
        <Button onClick={() => setShowOnboardModal(true)}>Onboard Organization</Button>
      </div>

      <div className={styles.searchBar}>
        <Input
          type="text"
          placeholder="Search organizations..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      <div className={styles.table}>
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Contact Email</th>
              <th>Phone</th>
              <th>Machines</th>
              <th>Active Tickets</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {filteredOrganizations.map(org => (
              <tr 
                key={org.id} 
                onClick={() => navigate(`/service/organizations/${org.id}`)}
                className={styles.clickableRow}
              >
                <td>{org.name}</td>
                <td>{org.contactEmail}</td>
                <td>{org.contactPhone || '-'}</td>
                <td>{org.machinesCount || 0}</td>
                <td>{org.activeTicketsCount || 0}</td>
                <td>
                  <Badge variant={org.serviceStatus === 'Active' ? 'success' : 'default'}>
                    {org.serviceStatus}
                  </Badge>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {showOnboardModal && (
        <OnboardOrganizationModal onClose={() => setShowOnboardModal(false)} />
      )}
    </div>
  );
};
```

**File:** `src/components/organizations/OrganizationList.module.css`

```css
.container {
  padding: 2rem;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header h1 {
  font-size: 2rem;
  font-weight: 600;
  color: var(--text-primary);
}

.searchBar {
  margin-bottom: 1.5rem;
  max-width: 400px;
}

.table {
  background: white;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.table table {
  width: 100%;
  border-collapse: collapse;
}

.table th {
  background-color: var(--background-secondary);
  padding: 1rem;
  text-align: left;
  font-weight: 600;
  color: var(--text-secondary);
  border-bottom: 1px solid var(--border-color);
}

.table td {
  padding: 1rem;
  border-bottom: 1px solid var(--border-color);
}

.clickableRow {
  cursor: pointer;
  transition: background-color 0.2s;
}

.clickableRow:hover {
  background-color: var(--background-hover);
}

@media (max-width: 768px) {
  .container {
    padding: 1rem;
  }

  .header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .table {
    overflow-x: auto;
  }
}
```

### Step 1.4: Create Onboard Organization Modal

**File:** `src/components/organizations/OnboardOrganizationModal.tsx`

```typescript
import React, { useState } from 'react';
import { Modal } from '../ui/Modal';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { useOrganizations } from '../../hooks/useOrganizations';
import styles from './OnboardOrganizationModal.module.css';

interface Props {
  onClose: () => void;
}

export const OnboardOrganizationModal: React.FC<Props> = ({ onClose }) => {
  const { createOrganization, isCreating } = useOrganizations();
  const [formData, setFormData] = useState({
    name: '',
    contactEmail: '',
    contactPhone: '',
    address: '',
    adminEmail: '',
    adminFirstName: '',
    adminLastName: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Clear error when user types
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const validate = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) newErrors.name = 'Organization name is required';
    if (!formData.contactEmail.trim()) newErrors.contactEmail = 'Contact email is required';
    if (!formData.adminEmail.trim()) newErrors.adminEmail = 'Admin email is required';
    if (!formData.adminFirstName.trim()) newErrors.adminFirstName = 'Admin first name is required';
    if (!formData.adminLastName.trim()) newErrors.adminLastName = 'Admin last name is required';

    // Email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (formData.contactEmail && !emailRegex.test(formData.contactEmail)) {
      newErrors.contactEmail = 'Invalid email format';
    }
    if (formData.adminEmail && !emailRegex.test(formData.adminEmail)) {
      newErrors.adminEmail = 'Invalid email format';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validate()) return;

    createOrganization(formData, {
      onSuccess: () => {
        onClose();
      },
    });
  };

  return (
    <Modal onClose={onClose} title="Onboard New Organization">
      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.section}>
          <h3>Organization Details</h3>
          
          <Input
            label="Organization Name *"
            name="name"
            value={formData.name}
            onChange={handleChange}
            error={errors.name}
            required
          />

          <Input
            label="Contact Email *"
            type="email"
            name="contactEmail"
            value={formData.contactEmail}
            onChange={handleChange}
            error={errors.contactEmail}
            required
          />

          <Input
            label="Contact Phone"
            type="tel"
            name="contactPhone"
            value={formData.contactPhone}
            onChange={handleChange}
            error={errors.contactPhone}
          />

          <Input
            label="Address"
            name="address"
            value={formData.address}
            onChange={handleChange}
            error={errors.address}
          />
        </div>

        <div className={styles.section}>
          <h3>Organization Administrator</h3>
          
          <Input
            label="Admin Email *"
            type="email"
            name="adminEmail"
            value={formData.adminEmail}
            onChange={handleChange}
            error={errors.adminEmail}
            required
          />

          <Input
            label="First Name *"
            name="adminFirstName"
            value={formData.adminFirstName}
            onChange={handleChange}
            error={errors.adminFirstName}
            required
          />

          <Input
            label="Last Name *"
            name="adminLastName"
            value={formData.adminLastName}
            onChange={handleChange}
            error={errors.adminLastName}
            required
          />
        </div>

        <div className={styles.actions}>
          <Button type="button" variant="secondary" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" disabled={isCreating}>
            {isCreating ? 'Onboarding...' : 'Onboard Organization'}
          </Button>
        </div>
      </form>
    </Modal>
  );
};
```

### Step 1.5: Create Organizations Page

**File:** `src/pages/service/OrganizationsPage.tsx`

```typescript
import React from 'react';
import { OrganizationList } from '../../components/organizations/OrganizationList';

export const OrganizationsPage: React.FC = () => {
  return <OrganizationList />;
};
```

### Step 1.6: Update App.tsx Routing

**File:** `src/App.tsx`

Update the organizations route from placeholder to actual page:

```typescript
import { OrganizationsPage } from './pages/service/OrganizationsPage';

// ... in routes
<Route
  path="/service/organizations"
  element={
    <ProtectedRoute requiredRole="service">
      <ServiceLayout>
        <OrganizationsPage />
      </ServiceLayout>
    </ProtectedRoute>
  }
/>
```

### Step 1.7: Create Organization Details Page (with tabs)

**File:** `src/pages/service/OrganizationDetailsPage.tsx`

```typescript
import React, { useState } from 'react';
import { useParams } from 'react-router-dom';
import { useOrganization } from '../../hooks/useOrganizations';
import { Loader } from '../../components/ui/Loader';
import { ErrorState } from '../../components/ui/ErrorState';
import { Card } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import styles from './OrganizationDetailsPage.module.css';

type TabType = 'info' | 'machines' | 'tickets' | 'users';

export const OrganizationDetailsPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { organization, isLoading, error } = useOrganization(id!);
  const [activeTab, setActiveTab] = useState<TabType>('info');

  if (isLoading) return <Loader />;
  if (error) return <ErrorState message="Failed to load organization" />;
  if (!organization) return <ErrorState message="Organization not found" />;

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <div>
          <h1>{organization.name}</h1>
          <Badge variant={organization.serviceStatus === 'Active' ? 'success' : 'default'}>
            {organization.serviceStatus}
          </Badge>
        </div>
        <Button onClick={() => {}}>Edit Organization</Button>
      </div>

      <div className={styles.tabs}>
        <button
          className={activeTab === 'info' ? styles.activeTab : ''}
          onClick={() => setActiveTab('info')}
        >
          Information
        </button>
        <button
          className={activeTab === 'machines' ? styles.activeTab : ''}
          onClick={() => setActiveTab('machines')}
        >
          Machines ({organization.machinesCount || 0})
        </button>
        <button
          className={activeTab === 'tickets' ? styles.activeTab : ''}
          onClick={() => setActiveTab('tickets')}
        >
          Tickets ({organization.activeTicketsCount || 0})
        </button>
        <button
          className={activeTab === 'users' ? styles.activeTab : ''}
          onClick={() => setActiveTab('users')}
        >
          Users
        </button>
      </div>

      <div className={styles.tabContent}>
        {activeTab === 'info' && (
          <Card>
            <div className={styles.infoGrid}>
              <div className={styles.infoItem}>
                <label>Contact Email:</label>
                <span>{organization.contactEmail}</span>
              </div>
              <div className={styles.infoItem}>
                <label>Contact Phone:</label>
                <span>{organization.contactPhone || '-'}</span>
              </div>
              <div className={styles.infoItem}>
                <label>Address:</label>
                <span>{organization.address || '-'}</span>
              </div>
              <div className={styles.infoItem}>
                <label>API Key:</label>
                <span className={styles.apiKey}>
                  {organization.apiKey ? '••••••••••••' : 'Not generated'}
                </span>
                <Button variant="secondary" size="small">Copy</Button>
                <Button variant="secondary" size="small">Regenerate</Button>
              </div>
            </div>
          </Card>
        )}

        {activeTab === 'machines' && (
          <div>Machines list will go here</div>
        )}

        {activeTab === 'tickets' && (
          <div>Tickets list will go here</div>
        )}

        {activeTab === 'users' && (
          <div>Users list will go here</div>
        )}
      </div>
    </div>
  );
};
```

---

## 🔨 PHASE 2: Machines Management (Continue from Organizations)

[Similar detailed instructions for machines...]

---

## 🔨 PHASE 3: Advanced Form Components

Before building tickets, create reusable form components:

### 1. Select Component
### 2. Textarea Component
### 3. DatePicker Component
### 4. DateRangePicker Component

[Detailed component specifications...]

---

## 🔨 PHASE 4: Tickets Management

[Detailed instructions...]

---

## 🧪 Testing Checklist

After each phase, test:

### Organizations:
- [ ] List organizations
- [ ] Search organizations
- [ ] Onboard new organization
- [ ] View organization details
- [ ] Update organization info
- [ ] Regenerate API key
- [ ] View organization machines
- [ ] View organization tickets
- [ ] View organization users

### Machines:
- [ ] List machines
- [ ] Add new machine
- [ ] View machine details
- [ ] Update machine info
- [ ] Change machine status
- [ ] View machine logs
- [ ] View machine tickets

### Tickets:
- [ ] List tickets (service)
- [ ] Filter tickets
- [ ] Search tickets
- [ ] View ticket details
- [ ] Change ticket status
- [ ] Assign ticket
- [ ] Create ticket (client)
- [ ] Update ticket description
- [ ] Reopen ticket (client admin)
- [ ] Add comment
- [ ] Edit/delete comment
- [ ] Add internal note
- [ ] Upload attachment
- [ ] Download attachment
- [ ] Delete attachment
- [ ] View timeline

### Dashboards:
- [ ] Service dashboard KPIs
- [ ] Service dashboard charts
- [ ] Client dashboard stats
- [ ] Activity feeds

### Team Management:
- [ ] List service users
- [ ] Invite service user
- [ ] Update service user
- [ ] Deactivate service user
- [ ] List organization users
- [ ] Invite organization user
- [ ] Remove organization user

---

## 📝 Code Quality Checklist

For each component/page:

- [ ] TypeScript types defined
- [ ] Props interface documented
- [ ] Error handling implemented
- [ ] Loading states shown
- [ ] Empty states handled
- [ ] Responsive design (mobile, tablet, desktop)
- [ ] Accessibility (ARIA labels, keyboard navigation)
- [ ] CSS modules for styling
- [ ] No console.log statements
- [ ] No hardcoded strings (use constants/i18n)
- [ ] Optimistic updates where appropriate
- [ ] React Query for server state
- [ ] Proper cleanup in useEffect
- [ ] Memoization where needed (useMemo, useCallback)

---

## 🚀 Deployment Checklist

Before deploying:

- [ ] All tests passing
- [ ] No TypeScript errors
- [ ] No ESLint errors
- [ ] Build successful
- [ ] Bundle size optimized (< 500KB gzipped)
- [ ] Lighthouse score > 90
- [ ] All environment variables configured
- [ ] API endpoints verified
- [ ] Error monitoring setup (optional)
- [ ] Analytics setup (optional)

---

**This guide should enable anyone (or an AI agent) to complete the implementation systematically and successfully.**
