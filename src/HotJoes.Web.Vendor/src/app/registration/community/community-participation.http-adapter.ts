import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { map, timeout } from "rxjs";
import {
  CommunityParticipationPort,
  CommunityParticipationResult,
  ContactPreference,
} from "./community-participation.port";

@Injectable()
export class HttpCommunityParticipationAdapter
implements CommunityParticipationPort {
  readonly #http = inject(HttpClient);

  join(vendorId: string, contactPreference: ContactPreference) {
    return this.#http.post<CommunityParticipationResult>(
      "/community-participations",
      { vendorId, contactPreference },
    ).pipe(
      timeout({ first: 15_000 }),
      map((result) => Object.freeze({ ...result })),
    );
  }
}
