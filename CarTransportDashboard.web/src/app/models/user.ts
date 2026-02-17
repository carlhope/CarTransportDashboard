import {TransportJob} from './transport-job';

export interface RegisterModel {
  email: string|null|undefined;
  password: string;
  fullName: string;
}

export interface LoginModel {
  email: string;
  password: string;
}

export interface UserModel {
  id: string;
  email: string;
  roles: string[];
  refreshToken?: string; // will be null after login
  accessToken?: string;
  csrfToken?: string;
}
export interface BaseUserRoleModel {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}
export interface DriverModel extends BaseUserRoleModel {
  licenseNumber: string;
}
export interface DispatcherModel extends BaseUserRoleModel {

}
export interface AdminModel extends BaseUserRoleModel {

}
export interface JwtPayload {
  email: string;
  fullName: string;
  roles: string[];
  exp: number;
}


