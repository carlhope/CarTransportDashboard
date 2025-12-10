export interface NavItem {
  label: string;
  route: string;
  roles: string[];
}

export const NAV_ITEMS: NavItem[] = [
  { label: 'Dashboard', route: '/dashboard', roles: ['Admin', 'Dispatcher', 'Driver'] },
  { label: 'Vehicles (not implemented)', route: '/vehicles', roles: ['Admin'] },
  { label: 'Transport Jobs', route: '/transport-jobs', roles: ['Dispatcher', 'Driver'] },
  { label: 'Drivers  (not implemented)', route: '/drivers', roles: ['Admin', 'Dispatcher'] },
  { label: 'Reports (not implemented)', route: '/reports', roles: ['Admin'] },
  { label: 'Earnings (not implemented)', route: '/earnings', roles: ['Driver'] },
  { label: 'Profile (not implemented)', route: '/profile', roles: ['Driver'] },
];
