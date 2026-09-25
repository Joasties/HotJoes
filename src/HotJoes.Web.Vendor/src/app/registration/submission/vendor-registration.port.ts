import { InjectionToken } from "@angular/core";
import { Observable } from "rxjs";
import { RegistrationDraft } from "../session/registration-session";

export interface RegistrationDeclarations {
  readonly authorisedToRegisterBusiness: boolean;
  readonly informationAccurate: boolean;
  readonly acceptHotJoesPlatformTerms: boolean;
}

export interface VendorRegistrationResult {
  readonly vendorId: string;
  readonly vendorState: string;
}

export interface VendorRegistrationPort {
  register(
    draft: RegistrationDraft,
    declarations: RegistrationDeclarations,
  ): Observable<VendorRegistrationResult>;
}

export const VENDOR_REGISTRATION = new InjectionToken<VendorRegistrationPort>(
  "VENDOR_REGISTRATION",
);
