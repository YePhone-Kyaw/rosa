export type User = {
    userId: number;
    name: string;
    email?: string;
    role: string;
    profilePicture?: string;
}

export type AuthResponse = {
    token: string;
    user: User;
}
