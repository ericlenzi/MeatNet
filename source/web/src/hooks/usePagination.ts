import { useState, useMemo } from 'react'

interface UsePaginationOptions {
  defaultPageSize?: number
}

export function usePagination({ defaultPageSize = 10 }: UsePaginationOptions = {}) {
  const [pageIndex, setPageIndex] = useState(0)
  const [pageSize, setPageSizeState] = useState(defaultPageSize)
  const [filter, setFilterState] = useState('')
  const [totalRows, setTotalRows] = useState(0)

  const totalPages = useMemo(
    () => Math.max(1, Math.ceil(totalRows / pageSize)),
    [totalRows, pageSize],
  )

  const setFilter = (value: string) => {
    setFilterState(value)
    setPageIndex(0)
  }

  const setPage = (index: number) => {
    setPageIndex(Math.max(0, Math.min(index, totalPages - 1)))
  }

  const setPageSize = (size: number) => {
    setPageSizeState(size)
    setPageIndex(0)
  }

  return {
    pageIndex,
    pageSize,
    filter,
    totalRows,
    totalPages,
    setFilter,
    setPage,
    setPageSize,
    setTotalRows,
  }
}
