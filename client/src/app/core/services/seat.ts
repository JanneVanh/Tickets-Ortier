import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Seat } from '../../shared/Models/Seat';
import { SeatStatus } from '../../shared/Models/SeatStatus';

@Injectable({
  providedIn: 'root'
})
export class SeatService {
  baseUrl = environment.apiUrl
  private http = inject(HttpClient)
  seats = signal<Seat[] | null>(null);

  getSeatsForShow(showId: number): Observable<Seat[]> {
    return this.http.get<Seat[]>(`${this.baseUrl}seat/show/${showId}`)
  }

  holdSeat(body: any): Observable<SeatStatus> {
    return this.http.post<SeatStatus>(`${this.baseUrl}seat/hold`, body);
  }  
  
  unholdSeat(body: any): Observable<SeatStatus> {
    return this.http.post<SeatStatus>(`${this.baseUrl}seat/unhold`, body);
  }
}
