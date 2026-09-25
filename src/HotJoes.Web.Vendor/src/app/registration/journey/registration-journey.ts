import { Component, inject } from "@angular/core";
import { Router, RouterLink, RouterOutlet } from "@angular/router";
import { RegistrationSessionStore } from "../session/registration-session.store";

@Component({
  selector: "app-registration-journey",
  imports: [RouterLink, RouterOutlet],
  providers: [RegistrationSessionStore],
  template: `
    <header>
      <a routerLink="/registration/start" class="brand">HotJoes</a>
      <span>Vendor registration</span>
    </header>
    <main>
      <nav aria-label="Registration progress">
        <ol>
          <li>Trading location</li>
          <li>Business identity</li>
          <li>Contact and address</li>
          <li>Trading characteristics</li>
          <li>Review</li>
        </ol>
      </nav>
      <router-outlet />
    </main>
  `,
  styles: [
    `
      :host {
        display: block;
        min-height: 100vh;
        padding: 1.5rem clamp(1rem, 4vw, 4rem);
        box-sizing: border-box;
      }
      header {
        clip: rect(0 0 0 0);
        clip-path: inset(50%);
        height: 1px;
        overflow: hidden;
        position: absolute;
        white-space: nowrap;
        width: 1px;
      }
      .brand {
        color: #ff8a00;
        font-size: 1.25rem;
        font-weight: 800;
        text-decoration: none;
      }
      main {
        padding-top: 1.5rem;
      }
      nav {
        clip: rect(0 0 0 0);
        clip-path: inset(50%);
        height: 1px;
        overflow: hidden;
        position: absolute;
        white-space: nowrap;
        width: 1px;
      }
    `,
  ],
})
export class RegistrationJourney {}

@Component({
  selector: "app-registration-start",
  template: `
    <section>
      <h1>Register your food business</h1>
      <p>
        We will guide you through the information needed to register with
        HotJoes.
      </p>
      <button type="button" (click)="begin()">Start registration</button>
    </section>
  `,
})
export class RegistrationStart {
  readonly #session = inject(RegistrationSessionStore);
  readonly #router = inject(Router);

  begin(): void {
    this.#session.start();
    void this.#router.navigate(["/registration/location"]);
  }
}

@Component({
  selector: "app-registration-business",
  imports: [RouterLink],
  template: `
    <section>
      <p class="eyebrow">Step 1 of 3</p>
      <h1>Your business</h1>
      <p>
        The typed business identity form belongs in the next implementation
        cohort.
      </p>
      <a routerLink="/registration/location">Continue to location</a>
    </section>
  `,
})
export class RegistrationBusinessStep {}

@Component({
  selector: "app-registration-location",
  imports: [RouterLink],
  template: `
    <section>
      <p class="eyebrow">Step 2 of 3</p>
      <h1>Your trading location</h1>
      <p>
        Location type and address selection will be added behind this route
        boundary.
      </p>
      <a routerLink="/registration/business">Back</a>
      <a routerLink="/registration/review">Continue to review</a>
    </section>
  `,
})
export class RegistrationLocationStep {}
