export interface NavItem {
  label: string;
  route: string;
  roles: string[];
}

export const NAV_ITEMS: NavItem[] = [
  { label: 'Dashboard', route: '/dashboard', roles: ['Admin', 'Dispatcher', 'Driver'] },
  { label: 'Vehicles', route: '/vehicles', roles: ['Admin'] },
  { label: 'Transport Jobs', route: '/transport-jobs', roles: ['Dispatcher', 'Driver'] },
  { label: 'Drivers', route: '/drivers', roles: ['Admin', 'Dispatcher'] },
  { label: 'Reports', route: '/reports', roles: ['Admin'] },
  { label: 'Earnings', route: '/earnings', roles: ['Driver'] },
  { label: 'Profile', route: '/profile', roles: ['Driver'] },
];
