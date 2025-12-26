import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationService } from '../../core/services/reservation';
import { ReservationSummary } from "../../shared/components/reservation-summary/reservation-summary";
import { TextInput } from "../../shared/components/text-input/text-input";
import { Show } from '../../shared/Models/Show';
import { ShowService } from '../../core/services/showService';

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
  shows: Show[] = []
  private showService = inject(ShowService)
  maxTickets: number = 10;

  constructor() {
    window.addEventListener('popstate', () => {
      this.reservationService.emptyReservation();
      this.router.navigate(['/ticketinfo']);
    });
  }

  // Custom validator for maximum tickets
  private maxTicketsValidator = (control: AbstractControl): ValidationErrors | null => {
    const numberOfAdults = parseInt(control.get('numberOfAdults')?.value) || 0;
    const numberOfChildren = parseInt(control.get('numberOfChildren')?.value) || 0;
    const totalTickets = numberOfAdults + numberOfChildren;
    const availableTickets = this.shows.find(s => s.id === this.showId)?.availableTickets ?? 10;
    this.maxTickets = availableTickets > 10 ? 10 : availableTickets;

    if (totalTickets > this.maxTickets) {
      return { maxTicketsExceeded: { max: this.maxTickets, actual: totalTickets } };
    }

    if (totalTickets === 0) {
      return { noTicketsSelected: true };
    }

    return null;
  }

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      const showIdParam = params.get('showid');
      this.showId = showIdParam !== null ? Number(showIdParam) : 0;
    })

    this.showService.getShows().subscribe(response => {
      this.shows = response
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
      seats: [],
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
  }, { validators: this.maxTicketsValidator })

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
        totalPrice: this.reservationService.totalPrice() ?? 0,
        seats: [],
      };

      this.reservationService.reservation.set(reservation);
      this.router.navigateByUrl(`/seatoverview/${this.showId}`);
    } else {
      this.reservationForm.markAllAsTouched();
    }
  }

  // Helper method to get form errors for display
  get totalTicketsError(): string | null {
    const formErrors = this.reservationForm.errors;
    if (formErrors?.['maxTicketsExceeded']) {
      return `Maximaal ${this.maxTickets} tickets toegestaan. U heeft ${formErrors['maxTicketsExceeded'].actual} tickets geselecteerd.`;
    }
    return null;
  }

  // Helper to check if form is valid for button state
  get isFormValid(): boolean {
    return this.reservationForm.valid;
  }

  // Helper to get total tickets for display
  get totalTickets(): number {
    const adults = parseInt(this.reservationForm.get('numberOfAdults')?.value ?? '') || 0;
    const children = parseInt(this.reservationForm.get('numberOfChildren')?.value ?? '') || 0;
    return adults + children;
  }
}

