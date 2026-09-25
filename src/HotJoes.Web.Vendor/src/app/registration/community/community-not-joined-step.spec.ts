import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { CommunityNotJoinedStep } from "./community-not-joined-step";

describe("CommunityNotJoinedStep", () => {
  it("AI-UX-009 confirms no participation and invents no operational link", async () => {
    await TestBed.configureTestingModule({
      imports: [CommunityNotJoinedStep],
      providers: [provideZonelessChangeDetection()],
    }).compileComponents();
    const fixture = TestBed.createComponent(CommunityNotJoinedStep);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain("Thank you");
    expect(fixture.nativeElement.textContent).toContain(
      "You can join the HotJoes community later",
    );
    expect(fixture.nativeElement.querySelector("a")).toBeNull();
  });
});
