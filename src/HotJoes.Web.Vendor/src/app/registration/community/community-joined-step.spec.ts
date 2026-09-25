import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { CommunityJoinedStep } from "./community-joined-step";

describe("CommunityJoinedStep", () => {
  it("AI-UX-009 presents the definitive joined outcome without a placeholder link", async () => {
    await TestBed.configureTestingModule({
      imports: [CommunityJoinedStep],
      providers: [provideZonelessChangeDetection()],
    }).compileComponents();
    const fixture = TestBed.createComponent(CommunityJoinedStep);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain(
      "Thank you for joining the HotJoes community",
    );
    expect(fixture.nativeElement.querySelector("a")).toBeNull();
  });
});
