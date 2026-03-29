import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { TransactionService } from '../../core/services/transaction.service';
import { Transaction, MonthlySummary, Category, CreateTransaction } from '../../shared/models/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  fullName = '';
  summary: MonthlySummary | null = null;
  transactions: Transaction[] = [];
  categories: Category[] = [];
  showForm = false;
  isLoading = true;

  selectedMonth: number = new Date().getMonth() + 1;
  selectedYear: number = new Date().getFullYear();

  readonly MONTH_NAMES = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];

  newTransaction: CreateTransaction = {
    categoryId: 0,
    amount: 0,
    transactionDate: new Date().toISOString().split('T')[0],
    description: ''
  };

  constructor(
    private authService: AuthService,
    private transactionService: TransactionService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.fullName = this.authService.getFullName() || 'User';
    this.loadCategories();
    this.loadData();
  }

  get monthLabel(): string {
    const now = new Date();
    const isCurrentMonth = this.selectedMonth === now.getMonth() + 1 && this.selectedYear === now.getFullYear();
    return isCurrentMonth
      ? `This Month (${this.MONTH_NAMES[this.selectedMonth - 1]} ${this.selectedYear})`
      : `${this.MONTH_NAMES[this.selectedMonth - 1]} ${this.selectedYear}`;
  }

  get isCurrentMonth(): boolean {
    const now = new Date();
    return this.selectedMonth === now.getMonth() + 1 && this.selectedYear === now.getFullYear();
  }

  prevMonth() {
    if (this.selectedMonth === 1) {
      this.selectedMonth = 12;
      this.selectedYear--;
    } else {
      this.selectedMonth--;
    }
    this.loadData();
  }

  nextMonth() {
    if (this.isCurrentMonth) return;
    if (this.selectedMonth === 12) {
      this.selectedMonth = 1;
      this.selectedYear++;
    } else {
      this.selectedMonth++;
    }
    this.loadData();
  }

  loadCategories() {
    this.transactionService.getCategories().subscribe({
      next: (data) => this.categories = data,
      error: () => {}
    });
  }

  loadData() {
    this.isLoading = true;

    this.transactionService.getMonthlySummary(this.selectedMonth, this.selectedYear).subscribe({
      next: (data) => {
        this.summary = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });

    this.transactionService.getTransactions(this.selectedMonth, this.selectedYear).subscribe({
      next: (data) => this.transactions = data,
      error: () => {}
    });
  }

  addTransaction() {
    if (!this.newTransaction.categoryId || !this.newTransaction.amount) {
      alert('Please fill in category and amount.');
      return;
    }

    const payload = {
      ...this.newTransaction,
      categoryId: +this.newTransaction.categoryId
    };

    this.transactionService.addTransaction(payload).subscribe({
        
      next: () => {
       
        this.showForm = false;
        this.newTransaction = {
          categoryId: 0,
          amount: 0,
          transactionDate: new Date().toISOString().split('T')[0],
          description: ''
        };
        this.loadData();
      },
      error: () => alert('Failed to add transaction.')
    });
  }

  deleteTransaction(id: number) {
    if (confirm('Delete this transaction?')) {
      this.transactionService.deleteTransaction(id).subscribe({
        next: () => this.loadData()
      });
    }
  }

  logout() {
    this.authService.logout();
  }
}

