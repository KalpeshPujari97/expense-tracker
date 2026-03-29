export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  fullName: string;
  email: string;
}

export interface Category {
  categoryId: number;
  name: string;
  type: string;
}

export interface CreateTransaction {
  categoryId: number;
  amount: number;
  transactionDate: string;
  description?: string;
}

export interface Transaction {
  transactionId: number;
  categoryName: string;
  categoryType: string;
  amount: number;
  transactionDate: string;
  description?: string;
  createdAt: string;
}

export interface MonthlySummary {
  totalIncome: number;
  totalExpense: number;
  balance: number;
  categoryBreakdown: CategorySummary[];
}

export interface CategorySummary {
  categoryName: string;
  categoryType: string;
  total: number;
}