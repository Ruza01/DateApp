import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Member } from '../_models/Member';
import { environment } from '../../environments/environment';
import { of, tap } from 'rxjs';
import { Photo } from '../_models/Photo';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  baseUrl = environment.apiUrl;
  //memebers = signal<Member[]>([]);
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);    //ovaj properti je neophodan za ono sto cemo vratiti sa servera  
  
  getMembers(pageNumber?: number, pageSize?: number) {
    let params = new HttpParams();

    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize',pageSize);
    }

    return this.http.get<Member[]>(this.baseUrl + '/users', {observe: 'response', params}).subscribe({
      next: response => {
        this.paginatedResult.set({
          items: response.body as Member[],
          pagination: JSON.parse(response.headers.get('Pagination')!)
        })
      }
    })
  }

  getMember(username: string) {
    // const member = this.memebers().find(x => x.userName === username);
    // if (member !== undefined) return of(member);  //mora of, jer treba da vraca observable, jer se u memeber-detail komponenti subscrajbujemo

    return this.http.get<Member>(this.baseUrl + '/users/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + '/users', member).pipe(
      // tap(() => { 
      //   this.memebers.update(members => members.map(m => m.userName === member.userName ? member : m))
      // })    
      //operator koji koristimo kada zelimo da koristimo spoljne efekte(interakcija sa bazom), ali ne zelimo da promenimo tok podataka
      //ovde kada updatujemo novog usera, koristimo tap, prolazimo kroz listu usera i trazimo onaj koji ima isti username sa onim kojeg
      //azuriramo i postavljamo mu vrednost na prosledjenu iz f-je, i tako updatujemo i signal
    )
  }

  setMainPhoto(photo: Photo) {
    return this.http.put(this.baseUrl + '/users/set-main-photo/' + photo.id, {}).pipe(
    //   tap(() => {
    //     this.memebers.update(members => members.map(m => {    //mora da se update-uje signal da bi se svuda videle promene, a ne tek kada osvezimo stranicu
    //       if (m.photos.includes(photo)) {
    //         m.photoUrl = photo.url
    //       }
    //       return m;
    //     }))
    //   })
     )
  }

  deletePhoto(photo: Photo) {
    return this.http.delete(this.baseUrl + '/users/delete-photo/' + photo.id).pipe(
      // tap(() => {
      //   this.memebers.update(members => members.map(m => {
      //     if (m.photos.includes(photo)) {
      //       m.photos = m.photos.filter(x => x.id !== photo.id)
      //     }
      //     return m;
      //   }))
      // })
    )
  }


}
