import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { map, Observable, timeout } from "rxjs";
import { RegistrationDraft } from "../session/registration-session";
import {
  ComplianceDetermination,
  LicenceDeterminationPort,
} from "./licence-determination.port";

@Injectable()
export class HttpLicenceDeterminationAdapter implements LicenceDeterminationPort {
  readonly #http = inject(HttpClient);

  determine(draft: RegistrationDraft): Observable<ComplianceDetermination> {
    return this.#http.post<ComplianceDetermination>(
      "/vendor-registration/required-licence-types",
      {
        legalOperatorType: toLowerCamel(draft.legalOperatorType),
        tradingLocation: toLowerCamel(draft.tradingLocation),
        weeklyOpeningHours: {
          days: draft.weeklyOpeningHours.map((hours) => ({
            day: hours.day.toLocaleLowerCase("en-GB"),
            isClosed: hours.isClosed,
            isOpenAllDay: hours.isOpenAllDay,
            startTime: toApiTime(hours.startTime),
            endTime: toApiTime(hours.endTime),
          })),
        },
        serviceIncludesHotFood: draft.serviceIncludesHotFood,
        alcoholService: draft.alcoholService,
        addressResolutionReference: draft.addressReference,
      },
    ).pipe(
      timeout({ first: 10_000 }),
      map(freezeDetermination),
    );
  }
}

const toLowerCamel = (value: string | null): string | null => value
  ? value.replace(/\s+(.)/g, (_match, character: string) => character.toUpperCase())
    .replace(/^./, (character) => character.toLowerCase())
  : null;

const toApiTime = (value: string | null): string | null => value
  ? value.length === 5 ? `${value}:00` : value
  : null;

const freezeDetermination = (value: ComplianceDetermination): ComplianceDetermination =>
  Object.freeze({
    ruleSetVersion: value.ruleSetVersion,
    items: Object.freeze(value.items.map((item) => Object.freeze({ ...item }))),
  });
