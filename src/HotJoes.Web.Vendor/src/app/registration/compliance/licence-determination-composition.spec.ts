import { TestBed } from "@angular/core/testing";
import { appConfig } from "../../app.config";
import { HttpLicenceDeterminationAdapter } from "./licence-determination.http-adapter";
import { LICENCE_DETERMINATION } from "./licence-determination.port";

describe("Required Licence Types composition", () => {
  it("binds the Web-client port to the HTTP adapter", () => {
    TestBed.configureTestingModule({
      providers: [...(appConfig.providers ?? [])],
    });

    expect(TestBed.inject(LICENCE_DETERMINATION))
      .toBeInstanceOf(HttpLicenceDeterminationAdapter);
  });
});
