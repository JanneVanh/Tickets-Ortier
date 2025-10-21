import { Show } from "./Show";

export type Reservation = {
    id: number;
    email: string;
    show: Show;
    numberOfAdults: number;
    numberOfChildren: number;
    paymentCode: string | null;
    isPaid: boolean;
}