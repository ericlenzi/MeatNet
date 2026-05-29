import axios from 'axios'
import type { LoginRequest, LoginResponse } from '@/types'

const baseURL = import.meta.env.VITE_API_BASE_URL

export async function login(request: LoginRequest): Promise<LoginResponse> {
  const response = await axios.post<LoginResponse>(
    `${baseURL}/Autenticacion`,
    request,
  )
  return response.data
}
