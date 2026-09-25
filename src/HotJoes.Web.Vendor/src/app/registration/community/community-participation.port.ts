import { InjectionToken } from "@angular/core";
import { Observable } from "rxjs";

export type ContactPreference = "email" | "sms" | "whatsApp";

export interface CommunityParticipationResult {
  readonly communityParticipationId: string;
  readonly vendorId: string;
  readonly contactPreference: ContactPreference;
  readonly joinedAt: string;
}

export interface CommunityParticipationPort {
  join(
    vendorId: string,
    contactPreference: ContactPreference,
  ): Observable<CommunityParticipationResult>;
}

export const COMMUNITY_PARTICIPATION =
  new InjectionToken<CommunityParticipationPort>(
    "COMMUNITY_PARTICIPATION",
  );
