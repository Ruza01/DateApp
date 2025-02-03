import { Component, input } from '@angular/core';
import { Member } from '../../_models/Member';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-member-card',
  imports: [RouterLink],
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css'
})
export class MemberCardComponent {

  member = input.required<Member>();  //prima od roditelja kad je input, kad je output, salje roditelju

}
