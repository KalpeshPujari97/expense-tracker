import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateTransaction, Transaction, MonthlySummary, Category } from '../../shared/models/models';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
  private apiUrl = 'https://localhost:44370/api';

  constructor(private http: HttpClient) {}

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/Categories`);
  }

  addTransaction(data: CreateTransaction): Observable<any> {
    return this.http.post(`${this.apiUrl}/Transactions`, data, { responseType: 'text' });
  }

  getTransactions(month?: number, year?: number): Observable<Transaction[]> {
    let url = `${this.apiUrl}/Transactions`;
    if (month && year) {
      url += `?month=${month}&year=${year}`;
    }
    return this.http.get<Transaction[]>(url);
  }

  deleteTransaction(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Transactions/${id}`, { responseType: 'text' });
  }

  getMonthlySummary(month?: number, year?: number): Observable<MonthlySummary> {
    let url = `${this.apiUrl}/Transactions/summary`;
    if (month && year) {
      url += `?month=${month}&year=${year}`;
    }
    return this.http.get<MonthlySummary>(url);
  }
}