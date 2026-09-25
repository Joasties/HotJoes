import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { timeout } from "rxjs";
import { RegistrationDraft } from "../session/registration-session";
import {
  RegistrationDeclarations,
  VendorRegistrationPort,
  VendorRegistrationResult,
} from "./vendor-registration.port";

const legalOperatorTypes = {
  "Sole Trader": "soleTrader",
  "General Partnership": "generalPartnership",
  "Limited Company": "limitedCompany",
  "Limited Liability Partnership": "limitedLiabilityPartnership",
  "Charitable Community Group": "charitableCommunityGroup",
  "Charitable Incorporated Organisation": "charitableIncorporatedOrganisation",
} as const;

const time = (value: string | null): string | null =>
  value && value.length === 5 ? `${value}:00` : value;

@Injectable()
export class HttpVendorRegistrationAdapter implements VendorRegistrationPort {
  readonly #http = inject(HttpClient);

  register(draft: RegistrationDraft, declarations: RegistrationDeclarations) {
    if (!draft.legalOperatorType || !draft.tradingLocation || !draft.addressReference) {
      throw new Error("The registration draft is incomplete.");
    }

    return this.#http.post<VendorRegistrationResult>("/vendors", {
      tradingName: draft.tradingName,
      legalOperatorName: draft.legalOperatorName,
      legalOperatorType: legalOperatorTypes[draft.legalOperatorType],
      companyRegistrationNumber: draft.companyRegistrationNumber || null,
      tradingCharacteristics: {
        tradingLocation: draft.tradingLocation.toLocaleLowerCase("en-GB"),
        weeklyOpeningHours: {
          days: draft.weeklyOpeningHours.map((hours) => ({
            day: hours.day.toLocaleLowerCase("en-GB"),
            isClosed: hours.isClosed,
            isOpenAllDay: hours.isOpenAllDay,
            startTime: time(hours.startTime),
            endTime: time(hours.endTime),
          })),
        },
        serviceIncludesHotFood: draft.serviceIncludesHotFood,
        alcoholService: draft.alcoholService,
      },
      primaryContact: {
        contactName: draft.contactName,
        contactEmail: draft.contactEmail,
        contactTelephone: draft.contactTelephone,
      },
      addressResolutionReference: draft.addressReference,
      website: draft.website || null,
      businessDescription: draft.businessDescription || null,
      registrationDeclarations: declarations,
    }).pipe(timeout({ first: 15_000 }));
  }
}
