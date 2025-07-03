export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPage: number;
}
export class PaginationResult<T> {
  items?: T;
  pagination?: Pagination;
}
