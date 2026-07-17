import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { LogIn, UserPlus, ShieldAlert, CheckCircle2 } from 'lucide-react';

const API_URL = 'http://localhost:5264/api';

export const AuthView: React.FC = () => {
  const { login, user, logout } = useAuth();
  const [isLogin, setIsLogin] = useState(true);

  // Form Fields
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [fullName, setFullName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [role, setRole] = useState('Member'); // default role

  // UI States
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const roles = [
    'Super Admin',
    'System Owner',
    'Branch Manager',
    'Receptionist',
    'Member',
    'Guest'
  ];

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);

    const payload = isLogin
      ? { usernameOrEmail: username || email, password }
      : { username, email, password, role, fullName, phoneNumber };

    const endpoint = isLogin ? '/auth/login' : '/auth/register';

    try {
      const res = await fetch(`${API_URL}${endpoint}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload),
      });

      if (!res.ok) {
        const errData = await res.json().catch(() => ({}));
        let errMsg = 'Something went wrong. Please check your inputs.';
        if (Array.isArray(errData)) {
          errMsg = errData.map(e => e.errorMessage || e.description).join(', ');
        } else if (errData.error) {
          errMsg = errData.error;
        } else if (errData.message) {
          errMsg = errData.message;
        }
        throw new Error(errMsg);
      }

      const data = await res.json();
      login(data.user, data.token);
      setSuccess(`Successfully ${isLogin ? 'logged in' : 'registered'}!`);
    } catch (err: any) {
      setError(err.message || 'Connection failed.');
    } finally {
      setLoading(false);
    }
  };

  if (user) {
    return (
      <div className="max-w-md mx-auto my-12 p-6 bg-white rounded-xl shadow-md text-center">
        <CheckCircle2 className="w-16 h-16 text-emerald-500 mx-auto mb-4" />
        <h2 className="text-2xl font-bold text-gray-800 mb-2">Welcome, {user.fullName}!</h2>
        <p className="text-gray-600 mb-4 font-medium">Role: <span className="text-blue-600">{user.role}</span></p>
        <p className="text-sm text-gray-500 mb-6">User ID: {user.id}</p>
        <button
          onClick={logout}
          className="w-full bg-red-500 hover:bg-red-600 text-white font-semibold py-2.5 rounded-lg transition"
        >
          Logout
        </button>
      </div>
    );
  }

  return (
    <div className="max-w-md mx-auto my-12 p-6 bg-white rounded-xl shadow-lg border border-gray-100">
      <div className="flex justify-center space-x-4 mb-6">
        <button
          onClick={() => { setIsLogin(true); setError(null); setSuccess(null); }}
          className={`flex-1 py-2 font-semibold text-center border-b-2 transition ${
            isLogin ? 'border-blue-600 text-blue-600' : 'border-transparent text-gray-400 hover:text-gray-600'
          }`}
        >
          Login
        </button>
        <button
          onClick={() => { setIsLogin(false); setError(null); setSuccess(null); }}
          className={`flex-1 py-2 font-semibold text-center border-b-2 transition ${
            !isLogin ? 'border-blue-600 text-blue-600' : 'border-transparent text-gray-400 hover:text-gray-600'
          }`}
        >
          Register
        </button>
      </div>

      <h2 className="text-2xl font-bold text-gray-800 text-center mb-6 flex items-center justify-center">
        {isLogin ? (
          <>
            <LogIn className="mr-2 h-6 w-6 text-blue-600" />
            Sign In
          </>
        ) : (
          <>
            <UserPlus className="mr-2 h-6 w-6 text-blue-600" />
            Create Account
          </>
        )}
      </h2>

      {/* Success Notification */}
      {success && (
        <div className="mb-4 p-3 bg-emerald-50 border border-emerald-200 text-emerald-700 rounded-lg flex items-center text-sm">
          <CheckCircle2 className="mr-2 h-5 w-5 flex-shrink-0" />
          {success}
        </div>
      )}

      {/* Error Notification */}
      {error && (
        <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg flex items-center text-sm">
          <ShieldAlert className="mr-2 h-5 w-5 flex-shrink-0" />
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Username */}
        <div>
          <label className="block text-sm font-semibold text-gray-700 mb-1">
            Username
          </label>
          <input
            type="text"
            required
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            className="w-full px-3.5 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
            placeholder="e.g. johndoe"
          />
        </div>

        {/* Email - Register Only */}
        {!isLogin && (
          <div>
            <label className="block text-sm font-semibold text-gray-700 mb-1">
              Email Address
            </label>
            <input
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full px-3.5 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
              placeholder="e.g. john@example.com"
            />
          </div>
        )}

        {/* Password */}
        <div>
          <label className="block text-sm font-semibold text-gray-700 mb-1">
            Password
          </label>
          <input
            type="password"
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full px-3.5 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
            placeholder="••••••••"
          />
        </div>

        {/* Full Name & Phone - Register Only */}
        {!isLogin && (
          <>
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-1">
                Full Name
              </label>
              <input
                type="text"
                required
                value={fullName}
                onChange={(e) => setFullName(e.target.value)}
                className="w-full px-3.5 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
                placeholder="e.g. John Doe"
              />
            </div>
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-1">
                Phone Number
              </label>
              <input
                type="text"
                value={phoneNumber}
                onChange={(e) => setPhoneNumber(e.target.value)}
                className="w-full px-3.5 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
                placeholder="e.g. +1 555-123-4567"
              />
            </div>
            <div>
              <label className="block text-sm font-semibold text-gray-700 mb-1">
                Select User Role
              </label>
              <select
                value={role}
                onChange={(e) => setRole(e.target.value)}
                className="w-full px-3.5 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm bg-white"
              >
                {roles.map(r => (
                  <option key={r} value={r}>{r}</option>
                ))}
              </select>
            </div>
          </>
        )}

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-2.5 rounded-lg transition flex items-center justify-center text-sm"
        >
          {loading ? (
            <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
            </svg>
          ) : null}
          {loading ? 'Processing...' : isLogin ? 'Sign In' : 'Sign Up'}
        </button>
      </form>
    </div>
  );
};
