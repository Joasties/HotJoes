import { TestBed } from "@angular/core/testing";
import { appConfig } from "../../app.config";
import { HttpRegisteredVendorAdapter } from "./registered-vendor.http-adapter";
import { REGISTERED_VENDOR_RETRIEVAL } from "./registered-vendor.port";

describe("Registered Vendor retrieval composition", () => {
  it("AI-WEB-003 binds the separate typed port to its HTTP adapter", () => {
    TestBed.configureTestingModule({
      providers: [...(appConfig.providers ?? [])],
    });

    expect(TestBed.inject(REGISTERED_VENDOR_RETRIEVAL))
      .toBeInstanceOf(HttpRegisteredVendorAdapter);
  });
});
