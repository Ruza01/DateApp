export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPagess: number;
}

export class PaginatedResult<T> {
    items?: T;
    pagination?: Pagination
}