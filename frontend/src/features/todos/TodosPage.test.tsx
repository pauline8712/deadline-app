import { render, screen } from '@testing-library/react'
import { afterEach, describe, expect, it, vi } from 'vitest'
import { TodosPage } from './TodosPage'

describe('TodosPage', () => {
  afterEach(() => {
    vi.unstubAllGlobals()
  })

  it('AK-3.5: shows an error message instead of an empty list when the API does not answer', async () => {
    vi.stubGlobal('fetch', vi.fn().mockRejectedValue(new TypeError('Failed to fetch')))

    render(<TodosPage />)

    expect(await screen.findByRole('alert')).toHaveTextContent(/kunde inte hämta uppgifterna/i)
    expect(screen.queryByText(/inga uppgifter än/i)).not.toBeInTheDocument()
  })
})
