import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { User } from '../_models/User';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  
  private http = inject(HttpClient);
  baseUrl = 'https://localhost:5001/api';
  currentUser = signal<User | null>(null);

  login(model: any) {
    return this.http.post<User>(this.baseUrl + '/account/login', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user)); //smesta korisnika u local storage kao string, to sluzi zbog osvezavanja stranice
          this.currentUser.set(user);
        } 
      })
    )
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + '/account/register', model).pipe(
      map(user => {
        if(user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUser.set(user);
        }
        return user;  //ovo mora da se doda,zato sto map fukcija trasformise resposne
      })
    )
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }


}
