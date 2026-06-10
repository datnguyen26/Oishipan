# Admin Dashboard - Visual Features

## 🎨 **Modern Design Components**

### **1. Dashboard Statistics Cards**

```
┌─────────────────────────────────────────────────────────────────┐
│                      Dashboard Stat Cards                        │
├─────────────────────────────────────────────────────────────────┤
│ ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐         │
│ │ 📦       │  │ 📋       │  │ 👥       │  │ 💰       │         │
│ │ Sản phẩm │  │ Đơn hàng │  │ Khách    │  │ Doanh    │         │
│ │          │  │          │  │ hàng     │  │ thu      │         │
│ │ 245      │  │ 89       │  │ 156      │  │ 45.2M₫   │         │
│ │ +5 mới   │  │ +12 hôm  │  │ +8.2%    │  │ +15.3%   │         │
│ └──────────┘  └──────────┘  └──────────┘  └──────────┘         │
└─────────────────────────────────────────────────────────────────┘
```

### **2. Sidebar Navigation**

```
┌──────────────────────────────┐
│  🎨 Gemini Admin             │
├──────────────────────────────┤
│  📊 Dashboard          (active)
│  📦 Sản phẩm
│  📂 Danh mục
│  🏷️  Thương hiệu
│  📋 Đơn hàng
│  👥 Người dùng
├──────────────────────────────┤
│  👤 Admin User               │
│  Quản trị viên               │
└──────────────────────────────┘
```

### **3. Data Table with Modern Styling**

```
┌─────────────────────────────────────────────────────────────┐
│  Sản phẩm mới nhất              [Xem tất cả →]              │
├─────────────────────────────────────────────────────────────┤
│  Tên sản phẩm        │ Giá       │ Danh mục   │ Thương hiệu │
├─────────────────────────────────────────────────────────────┤
│  iPhone 15 Pro       │ 29.99M₫   │ Điện thoại │ Apple       │
│  MacBook Air M3      │ 27.99M₫   │ Laptop     │ Apple       │
│  Galaxy S24 Ultra    │ 26.99M₫   │ Điện thoại │ Samsung     │
│  Bàn phím Keychron   │ 4.25M₫    │ Phụ kiện   │ Keychron    │
│  Tai nghe Sony       │ 6.49M₫    │ Âm thanh   │ Sony        │
└─────────────────────────────────────────────────────────────┘
```

### **4. Status Badges**

```
Badge Types:

[✓ Còn hàng]      - Green
[⚠ Sắp hết]       - Amber
[✕ Hết hàng]      - Red
[● Hoạt động]     - Blue
[● Bị khóa]       - Gray
```

### **5. Header with User Profile**

```
┌────────────────────────────────────────────────────────────┐
│ ☰  Dashboard                  🌙 🔔  👤 Admin  🚪         │
└────────────────────────────────────────────────────────────┘
```

---

## 🎯 **Color Palette**

### **Primary Colors**
- **Purple:** #7c3aed - Main brand color
- **Indigo:** #6366f1 - Secondary brand color

### **Semantic Colors**
- **Green:** #10b981 - Success/Active
- **Amber:** #f59e0b - Warning
- **Red:** #ef4444 - Danger/Error
- **Cyan:** #06b6d4 - Info

### **Background Colors**
- **Dark Base:** #0f172a
- **Dark Light:** #1e293b
- **Dark Lighter:** #334155
- **Transparent Border:** rgba(148, 163, 184, 0.15)

### **Text Colors**
- **Primary:** #f1f5f9
- **Secondary:** #cbd5e1
- **Muted:** #94a3b8

---

## ✨ **Animation Effects**

### **1. Card Hover Animation**
```
- Slight upward translation
- Border color change
- Shadow enhancement
- Smooth 0.3s transition
```

### **2. Button Hover**
```
- 2px upward movement
- Shadow expansion
- Color gradient shift
- Smooth transformation
```

### **3. Navigation Active State**
```
- Left border highlight
- Background gradient
- Smooth color transition
- Icon scaling
```

### **4. Toast Notifications**
```
- Slide-in animation (300ms)
- Fade-out animation (300ms)
- Auto-dismiss after 4 seconds
- Color-coded by type
```

---

## 📐 **Layout Specifications**

### **Desktop (1200px+)**
- Sidebar: 260px fixed
- Content padding: 32px
- Max-content width: Full responsive
- Cards grid: 4 columns

### **Tablet (768px - 1199px)**
- Sidebar: 260px (collapsed to icons on hover)
- Content padding: 24px
- Cards grid: 2 columns

### **Mobile (< 768px)**
- Sidebar: Bottom drawer
- Content padding: 16px
- Cards grid: 1 column
- Header height: 60px
- Bottom margin: 60px (for drawer)

---

## 🔤 **Typography**

### **Font Stack**
```css
-apple-system, 
BlinkMacSystemFont, 
'Segoe UI', 
'Roboto', 
'Oxygen', 
'Ubuntu', 
'Cantarell', 
sans-serif
```

### **Font Sizes**
- Page Title: 24px (Bold)
- Card Labels: 12px (Uppercase)
- Card Values: 32px (Bold)
- Table Headers: 12px (Uppercase)
- Body Text: 14px (Regular)
- Small Text: 12px (Regular)

### **Font Weights**
- Regular: 400
- Medium: 500
- Semibold: 600
- Bold: 700

---

## 🎭 **Component Examples**

### **Stat Card Structure**
```html
<div class="stat-card">
  <div class="stat-icon">
    <i class="bi bi-icon"></i>
  </div>
  <div class="stat-label">Label</div>
  <div class="stat-value">123</div>
  <p class="stat-desc">Description</p>
  <div class="stat-trend up">
    <i class="bi bi-arrow-up-right-circle-fill"></i>
    <span>+5%</span>
  </div>
</div>
```

### **Table Row Hover**
```css
/* Smooth background transition */
.table tbody tr:hover {
    background: rgba(124, 58, 237, 0.08);
    transition: background 0.3s ease;
}
```

### **Form Input Focus**
```css
.form-control:focus {
    background: rgba(30, 41, 59, 0.7);
    border-color: #7c3aed;
    box-shadow: 0 0 0 3px rgba(124, 58, 237, 0.1);
}
```

---

## 🌟 **Modern Features**

### **Glassmorphism**
- Backdrop blur effects
- Transparent backgrounds
- Layered depth effect

### **Gradient Text**
- Sidebar logo gradient
- Button gradients
- Smooth color transitions

### **Shadow Depth**
- Small shadows: Subtle elevation
- Medium shadows: Normal cards
- Large shadows: Modals & dropdowns

### **Smooth Transitions**
- All interactions: 0.3s cubic-bezier(0.4, 0, 0.2, 1)
- Timing function: Professional ease-in-out

---

## 🎨 **Dark Mode Advantages**

1. ✅ **Reduced Eye Strain** - Comfortable for extended use
2. ✅ **Battery Efficient** - OLED displays use less power
3. ✅ **Professional Look** - Modern SaaS aesthetic
4. ✅ **Focus** - Less visual noise, better concentration
5. ✅ **Accessibility** - Better contrast for many users

---

## 📊 **Responsive Behavior**

### **Breakpoints**

```
Mobile: < 480px
├─ Single column layout
├─ Compact buttons
├─ Hidden labels
└─ Bottom navigation

Tablet: 480px - 768px
├─ Two column layout
├─ Normal buttons
├─ Visible labels
└─ Side navigation (collapsed)

Desktop: 768px - 1200px
├─ Three column layout
├─ Large buttons
├─ Full labels
└─ Side navigation (visible)

Large Desktop: > 1200px
├─ Four column layout
├─ Full components
├─ All features visible
└─ Optimized spacing
```

---

## 🔄 **Interaction Patterns**

### **Hover States**
- Cards: Lift + Shadow
- Buttons: Opacity change
- Links: Underline/Color change
- Tables: Row highlight

### **Active States**
- Nav links: Border + Background
- Buttons: Pressed appearance
- Form inputs: Border color + Glow
- Badges: Inverted colors

### **Disabled States**
- Buttons: Opacity 0.5
- Inputs: Gray background
- Links: No pointer cursor
- Text: Muted color

---

## 🎯 **Best Practices Implemented**

✅ CSS Variables for easy theming  
✅ Semantic HTML structure  
✅ Proper color contrast (WCAG AA)  
✅ Keyboard navigation support  
✅ Touch-friendly button sizes (44px min)  
✅ Loading states and feedback  
✅ Error handling UI  
✅ Success notifications  

---

**Design System Version:** 1.0  
**Last Updated:** 2025  
**Compliance:** WCAG 2.1 AA

