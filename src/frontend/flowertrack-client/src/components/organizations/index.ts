// Stub components - to be implemented in a separate issue
import type {
  UpdateOrganizationRequest,
  OnboardOrganizationRequest,
  OrganizationDto,
} from '../../types/api';

interface EditOrganizationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: UpdateOrganizationRequest) => void;
  organization: OrganizationDto;
  isLoading: boolean;
}

export function EditOrganizationModal(_props: EditOrganizationModalProps) {
  return null;
}

interface OnboardOrganizationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (data: OnboardOrganizationRequest) => Promise<void>;
  isLoading: boolean;
}

export function OnboardOrganizationModal(_props: OnboardOrganizationModalProps) {
  return null;
}
