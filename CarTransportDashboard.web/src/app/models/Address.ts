export interface Address {
  companyName?: string;         // Optional business name
  addressLine1: string;         // Required: street address, building, etc.
  addressLine2?: string;        // Optional further detail
  locality: string;             // Town or city
  postalCode: string;
  country: string;

  lat?: number;
  lng?: number;
  formatted?: string;           // Full formatted address (e.g., from Google)
}

