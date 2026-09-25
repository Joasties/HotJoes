import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'registration',
    loadChildren: () =>
      import('./registration/registration.routes').then((feature) => feature.registrationRoutes),
  },
  { path: '', pathMatch: 'full', redirectTo: 'registration' },
  { path: '**', redirectTo: 'registration' },
];
