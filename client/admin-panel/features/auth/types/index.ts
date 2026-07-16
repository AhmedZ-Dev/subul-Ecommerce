export type AdminUserDto = {
  id: number;
  name: string;
  email: string;
  role: string;
};

export type LoginResponse = {
  accessToken: string;
  user: AdminUserDto;
};

export type GetCurrentUserResponse = {
  user: AdminUserDto;
};

export type LoginInput = {
  email: string;
  password: string;
};
