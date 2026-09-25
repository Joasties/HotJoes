import { provideZonelessChangeDetection } from "@angular/core";
import { ComponentFixture, TestBed } from "@angular/core/testing";
import { provideRouter, Router } from "@angular/router";
import { Subject } from "rxjs";
import {
  COMMUNITY_PARTICIPATION,
  CommunityParticipationPort,
  CommunityParticipationResult,
  ContactPreference,
} from "../community/community-participation.port";
import { RegistrationSessionStore } from "../session/registration-session.store";
import { RegistrationConfirmationStep } from "./registration-confirmation-step";

describe("RegistrationConfirmationStep", () => {
  let fixture: ComponentFixture<RegistrationConfirmationStep>;
  let adapter: RecordingCommunityParticipationPort;
  let router: Router;

  beforeEach(async () => {
    adapter = new RecordingCommunityParticipationPort();
    await TestBed.configureTestingModule({
      imports: [RegistrationConfirmationStep],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        RegistrationSessionStore,
        { provide: COMMUNITY_PARTICIPATION, useValue: adapter },
      ],
    }).compileComponents();

    const store = TestBed.inject(RegistrationSessionStore);
    store.start();
    store.updateBusinessIdentity({
      tradingName: "Hot Joes",
      legalOperatorName: "Hot Joes LLP",
      legalOperatorType: "Limited Liability Partnership",
      companyRegistrationNumber: "AB123456",
    });
    store.recordSubmission(
      "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
      "pendingActivation",
    );
    router = TestBed.inject(Router);
    vi.spyOn(router, "navigate").mockResolvedValue(true);
    fixture = TestBed.createComponent(RegistrationConfirmationStep);
    fixture.detectChanges();
  });

  it("AI-UX-009 starts with Email selected, opt-in unticked and Join disabled", () => {
    expect(fixture.componentInstance.form.getRawValue()).toEqual({
      contactPreference: "email",
      keepInvolved: false,
    });
    const radios = fixture.nativeElement.querySelectorAll(
      'input[type="radio"]',
    ) as NodeListOf<HTMLInputElement>;
    expect([...radios].map((radio) => radio.value)).toEqual([
      "email",
      "sms",
      "whatsApp",
    ]);
    expect(joinButton().disabled).toBe(true);
  });

  it("AI-UX-009 enables Join only after Keep me involved is checked", async () => {
    fixture.componentInstance.form.controls.keepInvolved.setValue(true);
    await fixture.whenStable();
    fixture.detectChanges();

    expect(joinButton().disabled).toBe(false);
  });

  it("VR-COMMUNITY-024 Not Now performs no request and shows its outcome", () => {
    clickButton("Not now");

    expect(adapter.calls).toEqual([]);
    expect(router.navigate).toHaveBeenCalledOnce();
    expect(router.navigate).toHaveBeenCalledWith([
      "/registration/community-not-joined",
    ]);
  });

  it("AI-WEB-010 submits the committed VendorId and selected preference once", async () => {
    fixture.componentInstance.form.setValue({
      contactPreference: "sms",
      keepInvolved: true,
    });
    await fixture.whenStable();
    fixture.detectChanges();

    const join = joinButton();
    join.click();
    fixture.detectChanges();
    join.click();
    fixture.detectChanges();

    expect(adapter.calls).toEqual([{
      vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
      contactPreference: "sms",
    }]);
    expect(join.disabled).toBe(true);
  });

  it("AI-UX-009 navigates only after definitive authoritative success", async () => {
    fixture.componentInstance.form.controls.keepInvolved.setValue(true);
    await fixture.whenStable();
    fixture.detectChanges();
    clickButton("Join the community");
    expect(router.navigate).not.toHaveBeenCalled();

    adapter.response.next(successResult);
    adapter.response.complete();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(router.navigate).toHaveBeenCalledOnce();
    expect(router.navigate).toHaveBeenCalledWith([
      "/registration/community-joined",
    ]);
  });

  it("AI-UX-009 retains selections and permits explicit retry after failure", async () => {
    fixture.componentInstance.form.setValue({
      contactPreference: "whatsApp",
      keepInvolved: true,
    });
    await fixture.whenStable();
    fixture.detectChanges();
    clickButton("Join the community");
    adapter.response.error({ status: 503 });
    await fixture.whenStable();
    fixture.detectChanges();

    expect(fixture.componentInstance.form.getRawValue()).toEqual({
      contactPreference: "whatsApp",
      keepInvolved: true,
    });
    expect(fixture.nativeElement.textContent).toContain(
      "Community participation is temporarily unavailable",
    );
    expect(joinButton().disabled).toBe(false);
  });

  function joinButton(): HTMLButtonElement {
    return [...fixture.nativeElement.querySelectorAll("button")]
      .find((button: HTMLButtonElement) =>
        button.textContent?.includes("Join the community"))!;
  }

  function clickButton(text: string): void {
    const button = [...fixture.nativeElement.querySelectorAll("button")]
      .find((candidate: HTMLButtonElement) =>
        candidate.textContent?.includes(text)) as HTMLButtonElement;
    button.click();
    fixture.detectChanges();
  }
});

class RecordingCommunityParticipationPort
implements CommunityParticipationPort {
  readonly calls: Array<{
    readonly vendorId: string;
    readonly contactPreference: ContactPreference;
  }> = [];
  response = new Subject<CommunityParticipationResult>();

  join(
    vendorId: string,
    contactPreference: ContactPreference,
  ) {
    this.calls.push({ vendorId, contactPreference });
    return this.response;
  }
}

const successResult: CommunityParticipationResult = {
  communityParticipationId: "11111111-2222-3333-4444-555555555555",
  vendorId: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
  contactPreference: "email",
  joinedAt: "2026-09-23T10:15:30.0000000Z",
};
