import { Routes } from "@angular/router";

export const routes: Routes = [
  {
    path: "registration",
    loadChildren: () =>
      import("./registration/registration.routes").then(
        (feature) => feature.registrationRoutes,
      ),
  },
  {
    path: "registered-vendors/:vendorId",
    loadComponent: () =>
      import("./vendor/retrieval/registered-vendor-details-step").then(
        (feature) => feature.RegisteredVendorDetailsStep,
      ),
  },
  { path: "", pathMatch: "full", redirectTo: "registration" },
  { path: "**", redirectTo: "registration" },
];
