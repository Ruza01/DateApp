import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams, HttpResponse } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/Message';
import { MembersService } from './members.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);

  private setPaginatedResponse(response: HttpResponse<Message[]>) {
    this.paginatedResult.set({
      items: response.body as Message[],
      pagination: JSON.parse(response.headers.get('Pagination')!)
    });
  }

  private setPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    
    if (pageNumber && pageSize) {
      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);
    }

    return params;
  }

  getMessage(pageNumber: number, pageSize: number, container: string) {
    let params = this.setPaginationHeaders(pageNumber, pageSize);

    params = params.append('Container', container);

    return this.http.get<Message[]>(this.baseUrl + '/messages', { observe: 'response', params }).subscribe({  //observe znaci da ne vrati samo body odgovora vec ceo httpResponse
      next: (response) => {
        this.setPaginatedResponse(response);
      }
    });
  }

  getMessageThread(usrename: string) {
    return this.http.get<Message[]>(this.baseUrl + '/messages/thread/' + usrename);
  }

  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + '/messages', {recepinetUsername: username, content});
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + '/messages/' + id);
  }
}
