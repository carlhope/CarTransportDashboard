export interface MinimalUser {
  //useful for providing basic user info without exposing sensitive details
  id: string;
  fullName: string;
  firstName?: string;
  lastName?: string;
  email?: string;
}
