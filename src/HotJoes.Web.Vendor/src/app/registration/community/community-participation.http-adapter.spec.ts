import { provideHttpClient } from "@angular/common/http";
import {
  HttpTestingController,
  provideHttpClientTesting,
} from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { HttpCommunityParticipationAdapter } from "./community-participation.http-adapter";

describe("HttpCommunityParticipationAdapter", () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      HttpCommunityParticipationAdapter,
      provideHttpClient(),
      provideHttpClientTesting(),
    ],
  }));

  it("AI-WEB-010 posts exactly VendorId and Contact Preference", () => {
    const adapter = TestBed.inject(HttpCommunityParticipationAdapter);
    const http = TestBed.inject(HttpTestingController);
    let actual: unknown;

    adapter.join(
      "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
      "whatsApp",
    ).subscribe((result) => { actual = result; });

    const request = http.expectOne("/community-participations");
    expect(request.request.method).toBe("POST");
    expect(request.request.body).toEqual({
      vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
      contactPreference: "whatsApp",
    });
    expect(Object.keys(request.request.body)).toEqual([
      "vendorId",
      "contactPreference",
    ]);

    request.flush({
      communityParticipationId: "11111111-2222-3333-4444-555555555555",
      vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
      contactPreference: "whatsApp",
      joinedAt: "2026-09-23T10:15:30.0000000Z",
    });

    expect(actual).toEqual({
      communityParticipationId: "11111111-2222-3333-4444-555555555555",
      vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
      contactPreference: "whatsApp",
      joinedAt: "2026-09-23T10:15:30.0000000Z",
    });
    expect(Object.isFrozen(actual)).toBe(true);
    http.verify();
  });

  it.each(["email", "sms", "whatsApp"] as const)(
    "maps the complete approved preference set: %s",
    (contactPreference) => {
      const adapter = TestBed.inject(HttpCommunityParticipationAdapter);
      const http = TestBed.inject(HttpTestingController);

      adapter.join(
        "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
        contactPreference,
      ).subscribe();

      const request = http.expectOne("/community-participations");
      expect(request.request.body.contactPreference).toBe(contactPreference);
      request.flush({
        communityParticipationId: "11111111-2222-3333-4444-555555555555",
        vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
        contactPreference,
        joinedAt: "2026-09-23T10:15:30.0000000Z",
      });
      http.verify();
    },
  );
});
