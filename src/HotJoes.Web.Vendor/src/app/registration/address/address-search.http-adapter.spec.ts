import { provideHttpClient } from "@angular/common/http";
import { HttpTestingController, provideHttpClientTesting } from "@angular/common/http/testing";
import { TestBed } from "@angular/core/testing";
import { HttpAddressSearchAdapter } from "./address-search.http-adapter";

describe("HttpAddressSearchAdapter", () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [HttpAddressSearchAdapter, provideHttpClient(), provideHttpClientTesting()],
  }));

  it("searches the Address boundary and maps only reference and display lines", () => {
    const adapter = TestBed.inject(HttpAddressSearchAdapter);
    const http = TestBed.inject(HttpTestingController);
    let result: unknown;

    adapter.search(" Greenwich ", "Stall").subscribe((value) => { result = value; });

    const request = http.expectOne(
      (candidate) => candidate.url === "/address-search"
        && candidate.params.get("query") === "Greenwich"
        && candidate.params.get("tradingLocation") === "stall",
    );
    request.flush({
      results: [{
        addressResolutionReference: "addr-stall-hotjoes-001",
        displayLines: ["Hot Joes Stall", "Greenwich Market", "London", "SE10 9HZ"],
      }],
    });

    expect(result).toEqual([{
      reference: "addr-stall-hotjoes-001",
      lines: ["Hot Joes Stall", "Greenwich Market", "London", "SE10 9HZ"],
    }]);
    http.verify();
  });
});
