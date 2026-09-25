import { TestBed } from "@angular/core/testing";
import { appConfig } from "../../app.config";
import { HttpCommunityParticipationAdapter } from "./community-participation.http-adapter";
import { COMMUNITY_PARTICIPATION } from "./community-participation.port";

describe("Community Participation composition", () => {
  it("AI-WEB-010 binds the typed port to the HTTP adapter", () => {
    TestBed.configureTestingModule({
      providers: [...(appConfig.providers ?? [])],
    });

    expect(TestBed.inject(COMMUNITY_PARTICIPATION))
      .toBeInstanceOf(HttpCommunityParticipationAdapter);
  });
});
