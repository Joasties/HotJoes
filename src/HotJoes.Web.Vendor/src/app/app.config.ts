import { ApplicationConfig, provideBrowserGlobalErrorListeners } from "@angular/core";
import { provideHttpClient } from "@angular/common/http";
import { provideRouter } from "@angular/router";
import { routes } from "./app.routes";
import { ADDRESS_SEARCH } from "./registration/address/address-search.port";
import { HttpAddressSearchAdapter } from "./registration/address/address-search.http-adapter";
import { VENDOR_REGISTRATION } from "./registration/submission/vendor-registration.port";
import { HttpVendorRegistrationAdapter } from "./registration/submission/vendor-registration.http-adapter";
import { LICENCE_DETERMINATION } from "./registration/compliance/licence-determination.port";
import { HttpLicenceDeterminationAdapter } from "./registration/compliance/licence-determination.http-adapter";
import { COMMUNITY_PARTICIPATION } from "./registration/community/community-participation.port";
import { HttpCommunityParticipationAdapter } from "./registration/community/community-participation.http-adapter";

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(),
    provideRouter(routes),
    { provide: ADDRESS_SEARCH, useClass: HttpAddressSearchAdapter },
    { provide: VENDOR_REGISTRATION, useClass: HttpVendorRegistrationAdapter },
    { provide: LICENCE_DETERMINATION, useClass: HttpLicenceDeterminationAdapter },
    { provide: COMMUNITY_PARTICIPATION, useClass: HttpCommunityParticipationAdapter },
  ],
};
