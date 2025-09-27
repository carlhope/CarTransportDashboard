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
  fullName: string;
  roles: string[];
  refreshToken?: string; // will be null after login
  accessToken?: string;
}
export interface DriverModel extends UserModel {
  licenseNumber: string;
  currentJob?: TransportJob;
}
export interface DispatcherModel extends UserModel {

}
export interface AdminModel extends UserModel {

}
export interface JwtPayload {
  email: string;
  fullName: string;
  roles: string[];
  exp: number;
}


