import { InjectionToken } from "@angular/core";
import { Observable } from "rxjs";

export interface RegisteredVendorDailyOpeningHours {
  readonly day: string;
  readonly isClosed: boolean;
  readonly isOpenAllDay: boolean;
  readonly startTime: string | null;
  readonly endTime: string | null;
}

export interface RegisteredVendorDetails {
  readonly vendorId: string;
  readonly registeredAt: string;
  readonly vendorState: string;
  readonly tradingPreference: string;
  readonly tradingName: string;
  readonly legalOperatorType: string;
  readonly legalOperatorName: string;
  readonly companyRegistrationNumber: string | null;
  readonly tradingCharacteristics: {
    readonly tradingLocation: string;
    readonly weeklyOpeningHours: {
      readonly days: readonly RegisteredVendorDailyOpeningHours[];
    };
    readonly serviceIncludesHotFood: boolean;
    readonly alcoholService: boolean;
  };
  readonly primaryContact: {
    readonly contactName: string;
    readonly contactEmail: string;
    readonly contactTelephone: string;
  };
  readonly canonicalAddressId: string;
  readonly businessAddressSnapshot: {
    readonly addressLine1: string;
    readonly addressLine2: string | null;
    readonly addressLine3: string | null;
    readonly postTown: string;
    readonly postcode: string;
    readonly county: string | null;
    readonly recipientOrOrganisationName: string | null;
  };
  readonly foodRegistrationAuthority: string;
  readonly primaryTradingAuthority: string | null;
  readonly website: string | null;
  readonly businessDescription: string | null;
}

export interface RegisteredVendorRetrievalPort {
  retrieve(vendorId: string): Observable<RegisteredVendorDetails>;
}

export const REGISTERED_VENDOR_RETRIEVAL =
  new InjectionToken<RegisteredVendorRetrievalPort>("REGISTERED_VENDOR_RETRIEVAL");
