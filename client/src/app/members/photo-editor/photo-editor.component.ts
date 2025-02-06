import { Component, inject, input, OnInit, output } from '@angular/core';
import { Member } from '../../_models/Member';
import { DecimalPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { FileUploader, FileUploadModule } from 'ng2-file-upload';
import { AccountService } from '../../_services/account.service';
import { environment } from '../../../environments/environment';
import { MembersService } from '../../_services/members.service';
import { Photo } from '../../_models/Photo';

@Component({
  selector: 'app-photo-editor',
  imports: [NgIf, NgFor, NgStyle,NgClass, FileUploadModule, DecimalPipe],
  templateUrl: './photo-editor.component.html',
  styleUrl: './photo-editor.component.css'
})
export class PhotoEditorComponent implements OnInit {
  private accountService = inject(AccountService);
  private memberService = inject(MembersService);
  member = input.required<Member>();
  uploader?: FileUploader;
  baseUrl = environment.apiUrl;
  memberChange = output<Member>();
  hasBaseDropZoneOver = false;  //dodao sam zbog html koda koji sam kopirao sa neta

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  deletePhoto(photo: Photo) {
    this.memberService.deletePhoto(photo).subscribe({
      next: () => {
        const updatedMember = {...this.member()}; //kopiramo membera
        updatedMember.photos = updatedMember.photos.filter(x => x.id !== photo.id);
        this.memberChange.emit(updatedMember);
      }
    })
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: () => {
        const user = this.accountService.currentUser(); //moramo korisniku da uploadujemo main sliku pored Welcome User
        if (user) {
          user.photoUrl = photo.url;
          this.accountService.setCurrentUser(user);
        }

        const updatedMember = {...this.member()};   //i da je postavimo za main u Edit profile
        updatedMember.photoUrl = photo.url;
        updatedMember.photos.forEach(p => {
          if (p.isMain) p.isMain = false;
          if (p.id == photo.id) p.isMain = true;
        });

        this.memberChange.emit(updatedMember);  //emituje roditelju
      }
    })
  }

  //slanje slika apiju
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + '/users/add-photo',
      authToken: 'Bearer ' + this.accountService.currentUser()?.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,  //10MB
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
      // Sprečava slanje cookies i CORS credentials sa zahtevom.
      // Ako API server ne podržava credentials, upload može pasti zbog CORS problema.
      // U većini slučajeva, ovo mora biti postavljeno kada API i frontend rade na različitim domenima.
    }

    this.uploader.onSuccessItem = (item, response, status, headers ) => {
      const photo = JSON.parse(response); //response je string sa servera, a mi ga konvertujemo u javascript objekat
      const updatedMember = {...this.member()};
      updatedMember.photos.push(photo);
      this.memberChange.emit(updatedMember);

    }
  }
}
