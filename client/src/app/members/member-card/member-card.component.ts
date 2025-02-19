import { Component, computed, inject, input } from '@angular/core';
import { Member } from '../../_models/Member';
import { RouterLink } from '@angular/router';
import { LikesService } from '../../_services/likes.service';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent {
  private likeService = inject(LikesService);
  member = input.required<Member>();  //prima od roditelja kad je input, kad je output, salje roditelju
  hasLiked = computed(() => this.likeService.likeIds().includes(this.member().id)); //computed je takodje signal koji prati promenu stanja

  toggleLike() {
    this.likeService.toggleLike(this.member().id).subscribe({
      next: () => {
        if (this.hasLiked() == true) {
          this.likeService.likeIds.update(ids => ids.filter(x => x !== this.member().id));  //brise lajk (unlike)
        } else {
          this.likeService.likeIds.update(ids => [...ids, this.member().id]);
        }
      }
    })
  }
}
