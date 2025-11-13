// Stub components - TODO: Implement organization modals
import type {
  OnboardOrganizationRequest,
  UpdateOrganizationRequest,
  OrganizationDto,
} from '../../types/api';

interface OnboardOrganizationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: OnboardOrganizationRequest) => Promise<void>;
  isLoading: boolean;
}

interface EditOrganizationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: UpdateOrganizationRequest) => void;
  organization: OrganizationDto;
  isLoading: boolean;
}

export const OnboardOrganizationModal = (_props: OnboardOrganizationModalProps) => null;
export const EditOrganizationModal = (_props: EditOrganizationModalProps) => null;
