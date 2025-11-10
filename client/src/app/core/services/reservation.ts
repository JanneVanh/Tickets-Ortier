import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Reservation } from '../../shared/Models/Reservation';

@Injectable({
  providedIn: 'root'
})
export class ReservationService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient)
  reservation = signal<Reservation | null>(null);
  priceForAdults = computed(() => {
    const reservation = this.reservation();
    if (!reservation || reservation.numberOfAdults == null) return null;
    return reservation.numberOfAdults * 14
  })
  priceForChildren = computed(() => {
    const reservation = this.reservation();
    if (!reservation || reservation.numberOfChildren == null) return null;
    return reservation.numberOfChildren * 7
  })
  totalPrice = computed(() => {
    const adultPrice = this.priceForAdults()
    const childPrice = this.priceForChildren()

    if (adultPrice === null && childPrice === null) return null

    return (adultPrice ?? 0) + (childPrice ?? 0)
  })
  totalSeats = computed(() => {
    const reservation = this.reservation();

    if (!reservation || ( reservation?.numberOfAdults === null && reservation.numberOfChildren === null)) return null

    return (reservation?.numberOfAdults ?? 0) + (reservation?.numberOfChildren ?? 0)
  })

  createReservation(values: any) {
    return this.http.post(this.baseUrl + 'reservation', values)
  }

  getReservations() {
    return this.http.get<Reservation[]>(this.baseUrl + 'reservation')
  }

  updateReservation(reservation: Reservation) {
    return this.http.put<Reservation>(this.baseUrl + 'reservation', reservation)
  }

  emptyReservation() {
    this.reservation.set(null)
  }
}
