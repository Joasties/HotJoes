import { provideHttpClient } from "@angular/common/http";
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from "@angular/core";
import { provideRouter, withInMemoryScrolling } from "@angular/router";
import { routes } from "./app.routes";
import { ADDRESS_SEARCH } from "./registration/address/address-search.port";
import { HttpAddressSearchAdapter } from "./registration/address/address-search.http-adapter";
import { COMMUNITY_PARTICIPATION } from "./registration/community/community-participation.port";
import { HttpCommunityParticipationAdapter } from "./registration/community/community-participation.http-adapter";
import { LICENCE_DETERMINATION } from "./registration/compliance/licence-determination.port";
import { HttpLicenceDeterminationAdapter } from "./registration/compliance/licence-determination.http-adapter";
import { VENDOR_REGISTRATION } from "./registration/submission/vendor-registration.port";
import { HttpVendorRegistrationAdapter } from "./registration/submission/vendor-registration.http-adapter";
import { HttpRegisteredVendorAdapter } from "./vendor/retrieval/registered-vendor.http-adapter";
import { REGISTERED_VENDOR_RETRIEVAL } from "./vendor/retrieval/registered-vendor.port";

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(),
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: "top",
        anchorScrolling: "enabled",
      }),
    ),
    { provide: ADDRESS_SEARCH, useClass: HttpAddressSearchAdapter },
    { provide: VENDOR_REGISTRATION, useClass: HttpVendorRegistrationAdapter },
    { provide: LICENCE_DETERMINATION, useClass: HttpLicenceDeterminationAdapter },
    { provide: COMMUNITY_PARTICIPATION, useClass: HttpCommunityParticipationAdapter },
    { provide: REGISTERED_VENDOR_RETRIEVAL, useClass: HttpRegisteredVendorAdapter },
  ],
};
