export const Toast = ({ toast }) => {
  if (!toast.show) return null;

  return (
    <div className="fixed bottom-6 right-6 z-50 animate-bounce">
      <div className={`flex items-center gap-3 px-5 py-3.5 rounded-2xl shadow-2xl border backdrop-blur-md ${
        toast.type === 'success' ? 'bg-slate-900/90 border-emerald-500/30 text-emerald-300' :
        toast.type === 'error' ? 'bg-slate-900/90 border-rose-500/30 text-rose-300' :
        'bg-slate-900/90 border-amber-500/30 text-amber-300'
      }`}>
        <span className="text-lg">
          {toast.type === 'success' ? '✨' : toast.type === 'error' ? '🛑' : '⚡'}
        </span>
        <div className="text-xs font-bold tracking-wide">{toast.message}</div>
      </div>
    </div>
  );
};

export default Toast;
