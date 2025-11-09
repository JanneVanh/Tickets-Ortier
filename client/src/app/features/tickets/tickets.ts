import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationService } from '../../core/services/reservation';
import { ReservationSummary } from "../../shared/components/reservation-summary/reservation-summary";
import { TextInput } from "../../shared/components/text-input/text-input";

@Component({
  selector: 'app-tickets',
  imports: [
    ReactiveFormsModule,
    TextInput,
    MatButton,
    ReservationSummary,
  ],
  templateUrl: './tickets.html',
  styleUrl: './tickets.scss'
})
export class Tickets implements OnInit {
  private activatedRoute = inject(ActivatedRoute)
  private fb = inject(FormBuilder)
  private reservationService = inject(ReservationService)
  validationErrors?: string[]
  showId: number = 0;
  private router = inject(Router)

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      const showIdParam = params.get('showid');
      this.showId = showIdParam !== null ? Number(showIdParam) : 0;
    })

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
      showId: this.showId!,
      numberOfAdults: formValue.numberOfAdults ? parseInt(formValue.numberOfAdults) : 0,
      numberOfChildren: formValue.numberOfChildren ? parseInt(formValue.numberOfChildren) : 0,
      paymentCode: null,
      email: "",
      isPaid: false,
      surName: "",
      name: "",
      remark: "",
      reservationDate: new Date().toISOString(),
      totalPrice: 0,
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

  onNext(): void {
    if (this. 
      reservationForm.valid) {
      const formValue = this.reservationForm.value;

      const reservation = {
        id: 0, // Will be assigned by backend
        showId: this.showId,
        numberOfAdults: formValue.numberOfAdults ? parseInt(formValue.numberOfAdults) : 0,
        numberOfChildren: formValue.numberOfChildren ? parseInt(formValue.numberOfChildren) : 0,
        paymentCode: null,
        email: formValue.email || "",
        isPaid: false,
        surName: formValue.lastName || "",
        name: formValue.firstName || "",
        remark: formValue.remark || "",
        reservationDate: new Date().toISOString(),
        totalPrice: 0, // Will be calculated by backend or service
      };

      this.reservationService.reservation.set(reservation);
      this.router.navigateByUrl(`/seatoverview/${this.showId}`);
    } else {
      this.reservationForm.markAllAsTouched();
    }
  }
}

