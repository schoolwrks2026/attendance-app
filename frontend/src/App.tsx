import React, { useState } from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import { AuthView } from './components/AuthView';
import { BranchView } from './components/BranchView';
import { MemberView } from './components/MemberView';
import { BookingView } from './components/BookingView';
import { Activity, Landmark, Dumbbell, Calendar, CheckSquare, BarChart3 } from 'lucide-react';

const AppContent: React.FC = () => {
  const { user, logout } = useAuth();
  const [activeTab, setActiveTab] = useState('auth');

  return (
    <div className="min-h-screen bg-gray-50 flex flex-col font-sans">
      {/* Responsive Header */}
      <header className="bg-white border-b border-gray-200 sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16 items-center">
            {/* Logo / Brand */}
            <div className="flex items-center space-x-3">
              <div className="bg-blue-600 text-white p-2.5 rounded-xl shadow-md">
                <Dumbbell className="h-6 w-6" />
              </div>
              <div>
                <h1 className="text-xl font-black text-gray-900 tracking-tight">Gym&Turf</h1>
                <p className="text-[10px] text-gray-500 font-semibold uppercase tracking-wider">Enterprise Management</p>
              </div>
            </div>

            {/* Profile / Logout */}
            {user && (
              <div className="flex items-center space-x-4">
                <div className="hidden md:block text-right">
                  <p className="text-sm font-bold text-gray-800">{user.fullName}</p>
                  <p className="text-[11px] text-blue-600 font-bold uppercase">{user.role}</p>
                </div>
                <button
                  onClick={logout}
                  className="bg-gray-100 hover:bg-gray-200 text-gray-700 px-3.5 py-1.5 rounded-lg text-sm font-semibold transition"
                >
                  Logout
                </button>
              </div>
            )}
          </div>
        </div>
      </header>

      {/* Main Container */}
      <main className="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Navigation Tabs */}
        {user && (
          <div className="flex overflow-x-auto pb-3 mb-6 space-x-2 border-b border-gray-200 scrollbar-none">
            <button
              onClick={() => setActiveTab('dashboard')}
              className={`flex items-center space-x-2 px-4 py-2 rounded-lg text-sm font-bold transition-all whitespace-nowrap ${
                activeTab === 'dashboard' ? 'bg-blue-600 text-white shadow-md' : 'bg-white hover:bg-gray-100 text-gray-600'
              }`}
            >
              <BarChart3 className="h-4 w-4" />
              <span>Dashboard</span>
            </button>
            <button
              onClick={() => setActiveTab('branches')}
              className={`flex items-center space-x-2 px-4 py-2 rounded-lg text-sm font-bold transition-all whitespace-nowrap ${
                activeTab === 'branches' ? 'bg-blue-600 text-white shadow-md' : 'bg-white hover:bg-gray-100 text-gray-600'
              }`}
            >
              <Landmark className="h-4 w-4" />
              <span>Branches</span>
            </button>
            <button
              onClick={() => setActiveTab('members')}
              className={`flex items-center space-x-2 px-4 py-2 rounded-lg text-sm font-bold transition-all whitespace-nowrap ${
                activeTab === 'members' ? 'bg-blue-600 text-white shadow-md' : 'bg-white hover:bg-gray-100 text-gray-600'
              }`}
            >
              <Activity className="h-4 w-4" />
              <span>Members</span>
            </button>
            <button
              onClick={() => setActiveTab('booking')}
              className={`flex items-center space-x-2 px-4 py-2 rounded-lg text-sm font-bold transition-all whitespace-nowrap ${
                activeTab === 'booking' ? 'bg-blue-600 text-white shadow-md' : 'bg-white hover:bg-gray-100 text-gray-600'
              }`}
            >
              <Calendar className="h-4 w-4" />
              <span>Turf & Court Booking</span>
            </button>
            <button
              onClick={() => setActiveTab('reception')}
              className={`flex items-center space-x-2 px-4 py-2 rounded-lg text-sm font-bold transition-all whitespace-nowrap ${
                activeTab === 'reception' ? 'bg-blue-600 text-white shadow-md' : 'bg-white hover:bg-gray-100 text-gray-600'
              }`}
            >
              <CheckSquare className="h-4 w-4" />
              <span>Reception Check-In</span>
            </button>
          </div>
        )}

        {/* Tab Content rendering */}
        <div className="space-y-6">
          {!user ? (
            <AuthView />
          ) : (
            <div>
              {activeTab === 'dashboard' && (
                <div className="p-6 bg-white rounded-xl shadow-md border border-gray-100 text-center">
                  <h3 className="text-xl font-bold text-gray-800">Operational Dashboard</h3>
                  <p className="text-gray-500 mt-2 text-sm">Welcome to Gym&Turf! Here you will manage memberships, facility bookings, and live reception check-ins.</p>
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mt-8">
                    <div className="p-4 bg-blue-50 rounded-xl border border-blue-100">
                      <span className="text-xs font-bold text-blue-600 uppercase tracking-wider">Gym Members</span>
                      <p className="text-3xl font-black text-blue-900 mt-1">24 Active</p>
                    </div>
                    <div className="p-4 bg-emerald-50 rounded-xl border border-emerald-100">
                      <span className="text-xs font-bold text-emerald-600 uppercase tracking-wider">Bookings Today</span>
                      <p className="text-3xl font-black text-emerald-900 mt-1">8 Slots</p>
                    </div>
                    <div className="p-4 bg-amber-50 rounded-xl border border-amber-100">
                      <span className="text-xs font-bold text-amber-600 uppercase tracking-wider">Checked In</span>
                      <p className="text-3xl font-black text-amber-900 mt-1">3 Checked In</p>
                    </div>
                  </div>
                </div>
              )}

              {activeTab === 'branches' && (
                <BranchView />
              )}

              {activeTab === 'members' && (
                <MemberView />
              )}

              {activeTab === 'booking' && (
                <BookingView />
              )}

              {activeTab === 'reception' && (
                <div className="p-6 bg-white rounded-xl shadow-md border border-gray-100 text-center">
                  <h3 className="text-xl font-bold text-gray-800">Reception Desk</h3>
                  <p className="text-gray-500 mt-1 text-sm">This module is coming up in Module 5.</p>
                </div>
              )}
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

export default App;
