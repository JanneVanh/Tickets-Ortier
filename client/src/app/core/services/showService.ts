import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Show } from '../../shared/Models/Show';

@Injectable({
  providedIn: 'root'
})
export class ShowService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient)

  getShows(): Observable<Show[]> {
    return this.http.get<Show[]>(this.baseUrl + 'show')
  }

  getShowsWithTickets(): Observable<Show[]> {
    return this.http.get<Show[]>(this.baseUrl + 'show/withtickets')
  }
}
