export interface MinimalUser {
  //useful for providing basic user info without exposing sensitive details
  id: string;
  fullName: string;
  email?: string;
}
