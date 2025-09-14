import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationService } from '../../core/services/reservation';
import { Snackbar } from '../../core/services/snackbar';
import { ReservationSummary } from "../../shared/components/reservation-summary/reservation-summary";
import { TextInput } from "../../shared/components/text-input/text-input";
import { Show } from '../../shared/Models/Show';

@Component({
  selector: 'app-tickets',
  imports: [
    ReactiveFormsModule,
    TextInput,
    MatButton,
    ReservationSummary
  ],
  templateUrl: './tickets.html',
  styleUrl: './tickets.scss'
})
export class Tickets implements OnInit {
  private activatedRoute = inject(ActivatedRoute)
  private fb = inject(FormBuilder)
  private reservationService = inject(ReservationService)
  private router = inject(Router)
  private snack = inject(Snackbar)
  validationErrors?: string[]
  show: Show | undefined;

  ngOnInit(): void {
    this.setupFormSubscriptions();
  }

  setupFormSubscriptions() {
    // Subscribe to form changes and update reservation immediately
    this.reservationForm.valueChanges.subscribe(formValue => {
      this.updateReservation(formValue);
    });

    // Initialize reservation with current form values
    this.updateReservation(this.reservationForm.value);
  }

  updateReservation(formValue: any) {
    // Create a temporary reservation object for price calculation
    const tempReservation = {
      id: 0, // Temporary ID for form preview
      show: this.show!,
      numberOfAdults: formValue.ticketsAdults ? parseInt(formValue.ticketsAdults) : 0,
      numberOfChildren: formValue.ticketsChildren ? parseInt(formValue.ticketsChildren) : 0,
      paymentCode: null
    };
    this.reservationService.reservation.set(tempReservation);
  }

  reservationForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    numberOfAdults: [''],
    numberOfChildren: [''],
    remark: [''],
  })

  onSubmit() {
    const id = this.activatedRoute.snapshot.paramMap.get('showid')
    if (!id) return;

    const reservationData = {
      SurName: this.reservationForm.value.lastName,
      Name: this.reservationForm.value.firstName,
      Email: this.reservationForm.value.email,
      NumberOfAdults: parseInt(this.reservationForm.value.numberOfAdults || '0'),
      NumberOfChildren: parseInt(this.reservationForm.value.numberOfChildren || '0'),
      ShowId: parseInt(id),
      Remark: this.reservationForm.value.remark || null,
      ReservationDate: new Date().toISOString(),
      TotalPrice: this.reservationService.totalPrice(),
      IsPaid: false
    };

    this.reservationService.createReservation(reservationData).subscribe({
      next: (data: any) => {
        this.reservationService.reservation.set(data);
        this.snack.success('Je tickets zijn gereserveerd.');
        this.router.navigateByUrl('/confirmation');
      },
      error: (error) => {
        console.error('Reservation failed:', error);
        console.error('Error details:', error.error);
        this.snack.error('Er is een fout opgetreden bij het reserveren.');
      }
    });
  }
}
